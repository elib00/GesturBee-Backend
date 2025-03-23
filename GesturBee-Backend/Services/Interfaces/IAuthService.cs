using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDTO<UserDetailsDTO>> ValidateUser(UserValidationDTO credentials);
        Task<ApiResponseDTO<UserDetailsDTO>> RegisterUser(UserRegistrationDTO userDetails);
    }
}
