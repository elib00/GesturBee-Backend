using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GesturBee_Backend.Services
{
    public class FacebookAuthService : IExternalAuthService
    {
        public AuthenticationProperties GetAuthProperties()
        {
            return new AuthenticationProperties { RedirectUri = "https://localhost:7152/api/auth/facebook-callback/" };
        }

        public async Task<Dictionary<string, string>> GetUserInfoAsync(HttpContext httpContext)
        {
            var authenticateResult = await httpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
            {
                throw new Exception("Authentication failed");
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var userInfo = new Dictionary<string, string>
        {
            { "Name", claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "N/A" },
            { "Email", claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "N/A" }
        };

            return userInfo;
        }
    }
}
