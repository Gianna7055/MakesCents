/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/6962a61c-d0f8-8333-99ec-aee778f83ea2
 */
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MakesCentsBackend.Services.Utilities
{
    /// <summary>
    /// Service responsible for generating JWT tokens
    /// </summary>
    public class JwtService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly int _expiresMinutes;

        /// <summary>
        /// Set readonly class variables
        /// </summary>
        /// <param name="configuration"></param>
        /// <exception cref="Exception"></exception>
        public JwtService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"] ?? throw new Exception("JWT Key missing");
            _issuer = configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer missing");
            _expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var mins) ? mins : 60;
        }

        // Generate a JWT token for a given userId
        public string GenerateToken(int userId)
        {
            // 1. Create claims for this token
            // Claims are pieces of information stored in the token
            var claims = new[]
            {
            // 'sub' (subject) claim: identifies the user this token is for
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),

            // 'jti' (JWT ID) claim: unique identifier for this token (helps prevent replay attacks)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // 2. Convert the secret key string to a cryptographic key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_key) // Secret must be UTF-8 bytes
            );

            // 3. Create signing credentials
            // HMACSHA256 algorithm will sign the token with the secret key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _issuer,              // Who issued the token
                audience: null,                        // Optional, who can accept it (null for now)
                claims: claims,                        // The claims defined above
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes), // Token expiration
                signingCredentials: creds              // How the token is signed
            );

            // 5. Serialize the JWT to a compact string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
