using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(AuthTokenRequestDTO details);
        ResponseType ValidatePasswordResetToken(string token);
    }
}
