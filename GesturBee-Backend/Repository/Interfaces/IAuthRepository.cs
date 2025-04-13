using GesturBee_Backend.Models;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> IsExistingEmail(string email);
        Task<User?> GetUserByEmail(string email);
        Task CreateUser(User user);
        Task<bool> IsUserAStudent(int userId);
        Task<bool> IsUserATeacher(int userId);
        Task ResetPassword(string email, string newPassword);
        Task UpdateLastLogin(User user);
    }
}
