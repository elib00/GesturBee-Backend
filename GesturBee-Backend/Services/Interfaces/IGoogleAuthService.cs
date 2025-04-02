using Microsoft.AspNetCore.Authentication;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IGoogleAuthService
    {
        AuthenticationProperties GetAuthProperties();
        Task<Dictionary<string, string>> GetUserInfoAsync(HttpContext httpContext);
    }
}
