using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ApiResponseDTO<UserProfileDTO>> EditUserProfile(UserProfileDTO profile);
    }
}
