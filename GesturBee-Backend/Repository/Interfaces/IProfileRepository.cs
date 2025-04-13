using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IProfileRepository
    {
        Task<UserProfileDTO> EditUserProfile(UserProfileDTO profile);
    }
}
