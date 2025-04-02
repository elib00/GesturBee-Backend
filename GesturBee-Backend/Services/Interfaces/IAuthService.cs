using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDTO<UserDetailsDTO>> ValidateUser(UserValidationDTO credentials);
        Task<ApiResponseDTO<UserDetailsDTO>> RegisterUser(UserRegistrationDTO userDetails, AuthType authType);
        Task<ApiResponseDTO<UserDetailsDTO>> FetchUserUsingEmail(string email);
    }
}
