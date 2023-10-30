using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Contract.Generators
{
    public class TokenGenerator
    {
        private string _clientId;
        private string _clientSecret;
        private string _refreshToken;

        public TokenGenerator(string clientId, string clientSecret, string refreshToken)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _refreshToken = refreshToken;
        }

        public Token GenerateAccessToken(IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expirationTime = DateTime.UtcNow.AddMinutes(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _clientId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expirationTime).ToUnixTimeSeconds().ToString())
                }),
                Expires = expirationTime,
                SigningCredentials = credentials,
                Issuer = configuration["JWT:ValidIssuer"],
                Audience = configuration["JWT:ValidAudience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new Token
            {
                access_token = tokenHandler.WriteToken(token),
                refresh_token = _refreshToken,
                expiration_time = expirationTime,
            };
        }

        public async Task<Token> GenerateAccessTokenAsync(IConfiguration configuration)
        {
            return await Task.Run(() => GenerateAccessToken(configuration));
        }

        public class Token
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public DateTime expiration_time { get; set; }
        }
    }
}
