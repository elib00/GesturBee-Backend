using GesturBee_Backend.Models;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IAuthRepository
    {
        public Task<bool> IsExistingEmail(string email);
        public Task<User?> GetUserByEmail(string email);
        public Task CreateUser(User user);
    }
}
