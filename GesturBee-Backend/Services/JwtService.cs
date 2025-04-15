using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
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


             //add the type of token
             claims.Add(new Claim("type", details.Type));

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

        public ResponseType ValidatePasswordResetToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                }, out var validatedToken);

                //Console.WriteLine($"Token validated: {validatedToken}");

                // Check if token has the "password-reset" claim and it's not expired
                var tokenType = claimsPrincipal.FindFirst("type")?.Value;
                if (tokenType == "password-reset")
                {
                    return ResponseType.ValidToken;
                }

                return ResponseType.TokenMissingRequiredClaim; //not a password reset token
            }
            catch (Exception)
            {
                return ResponseType.InvalidToken; //invalid token
            }
        }
    }
}
