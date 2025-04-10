using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Enums;

namespace GesturBee_Backend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<ApiResponseDTO<UserProfileDTO>> EditUserProfile(UserProfileDTO profile)
        {
            UserProfileDTO updatedProfile = await _profileRepository.EditUserProfile(profile);

            if(updatedProfile == null)
            {
                return new ApiResponseDTO<UserProfileDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.ProfileNotFound,
                    Data = null
                };
            }

            return new ApiResponseDTO<UserProfileDTO>
            {
                Success = true,
                ResponseType = ResponseType.ProfileUpdated,
                Data = updatedProfile
            };
        }
    }
}
