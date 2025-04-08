using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GesturBee_Backend.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(AuthTokenRequestDTO details)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, details.Email),
                new Claim(JwtRegisteredClaimNames.Email, details.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };



            //add each role manually
            foreach (var role in details.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string ValidatePasswordResetToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"));
                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "yourdomain.com",
                    ValidateAudience = true,
                    ValidAudience = "yourdomain.com",
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                }, out var validatedToken);

                // Check if token has the "password-reset" claim and it's not expired
                var tokenType = claimsPrincipal.FindFirst("type")?.Value;
                if (tokenType == "password-reset")
                {
                    return claimsPrincipal.Identity.Name; // return user ID or similar claim
                }

                return null; // Invalid token
            }
            catch (Exception)
            {
                return null; // Invalid token
            }
        }
    }
}
