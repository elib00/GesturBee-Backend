using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(AuthTokenRequestDTO details);
        string ValidatePasswordResetToken(string token);
    }
}
