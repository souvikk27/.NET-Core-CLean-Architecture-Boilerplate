
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.OpenAPI.Auth.Services
{
    public class PKCEGenerator
    {
        private static readonly RandomNumberGenerator RandomGenerator = RandomNumberGenerator.Create();

        public static string GenerateCodeVerifier()
        {
            byte[] buffer = new byte[32]; // 32 bytes for a 256-bit code verifier
            RandomGenerator.GetBytes(buffer);
            return Base64UrlEncoder.Encode(buffer);
        }

        public static string TransformCodeVerifier(string codeVerifier, PKCETransformationMethod method)
        {
            if (method == PKCETransformationMethod.S256)
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                    return Base64UrlEncoder.Encode(hashedBytes);
                }
            }
            else if (method == PKCETransformationMethod.Plain)
            {
                return codeVerifier;
            }

            throw new ArgumentException("Invalid PKCE transformation method");
        }
    }
}

public enum PKCETransformationMethod
{
    Plain,
    S256
}
