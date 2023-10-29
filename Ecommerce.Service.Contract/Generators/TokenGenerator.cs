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

        public Token GenerateAccessToken()
        {
            var expirationTime = DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _clientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, expirationTime)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_clientSecret));

            var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            var payload = new JwtPayload(claims);

            var jwt = new JwtSecurityToken(header, payload);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new Token
            {
                access_token = token,
                refresh_token = _refreshToken,
                expiration_time = expirationTime,
            };
        }

        public async Task<Token> GenerateAccessTokenAsync()
        {
            return await Task.Run(() => GenerateAccessToken());
        }

        public class Token
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string expiration_time { get; set; }
        }
    }
}
