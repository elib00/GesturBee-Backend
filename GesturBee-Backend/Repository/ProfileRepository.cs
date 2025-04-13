using GesturBee_Backend.DTO;
using GesturBee_Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GesturBee_Backend.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly BackendDbContext? _backendDbContext;

        public ProfileRepository(BackendDbContext? backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }
        public async Task<UserProfileDTO> EditUserProfile(UserProfileDTO profile)
        {
            var userProfileFromDb = await _backendDbContext.UserProfiles.FindAsync(profile.UserProfileId);

            if(userProfileFromDb == null)
            {
                return null;
            }

            //update
            userProfileFromDb.FirstName = profile.FirstName;
            userProfileFromDb.LastName = profile.LastName;
            userProfileFromDb.ContactNumber = profile.ContactNumber;
            userProfileFromDb.Gender = profile.Gender;
            userProfileFromDb.BirthDate = profile.BirthDate;

            await _backendDbContext.SaveChangesAsync();

            return new UserProfileDTO
            {
                FirstName = userProfileFromDb.FirstName,
                LastName = userProfileFromDb.LastName,
                ContactNumber = userProfileFromDb.ContactNumber,
                Gender = userProfileFromDb.Gender,
                BirthDate = userProfileFromDb.BirthDate
            };
        }
    }
}
