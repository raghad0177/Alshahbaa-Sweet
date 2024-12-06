using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace alshahbaasweets.DTO
{
    public class TokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string username)
        {
            // Hardcoded key (use secure storage in production)
            var secretKey = "rthjkljhgfuiosdfghjhgfdsiuytre4564EREFDsds67ffredd7";

            // Claims for the token
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username)
    };

            // Generate the symmetric security key
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Create the token without specifying issuer and audience
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // Return the token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
