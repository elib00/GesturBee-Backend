﻿using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDTO<UserDetailsDTO>> ValidateUser(UserValidationDTO credentials);
        Task<ApiResponseDTO<UserDetailsDTO>> RegisterUser(UserRegistrationDTO userDetails);
        Task<ApiResponseDTO<UserDetailsDTO>> ProcessGoogleAuth(Dictionary<string, string> userInfo);
        Task<ApiResponseDTO<UserDetailsDTO>> ProcessFacebookAuth(FacebookUserInfoDTO userInfo);
        Task<ApiResponseDTO> ResetPassword(ResetPasswordDTO resetDetails);
    }
}
