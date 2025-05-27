using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GesturBee_Backend.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BackendDbContext? _backendDbContext;

        public AuthRepository(BackendDbContext? backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _backendDbContext.Users
               .Include(user => user.Account)
               .Include(user => user.Profile)
               .FirstOrDefaultAsync(user => user.Account.Email == email);
        }

        public async Task<bool> IsExistingEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _backendDbContext.UserAccounts
                .AsNoTracking()
                .AnyAsync(userAccount => userAccount.Email == email);
        }

        public async Task<bool> CreateUser(User user)
        {
            try
            {
                await _backendDbContext.Users.AddAsync(user);
                await _backendDbContext.SaveChangesAsync();
                return true;
            }catch(DbUpdateException)
            {
                return false;
            }
        }

        public async Task ResetPassword(string email, string newPassword)
        {
            await _backendDbContext.UserAccounts
                .Where(userAccount => userAccount.Email == email)
                .ExecuteUpdateAsync(setter => setter.SetProperty(userAccount => userAccount.Password, newPassword));
        }

        public async Task UpdateLastLogin(User user)
        {
            user.LastLogin = DateTime.Now;
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task CreateRoadmapProgress(RoadmapProgress roadmapProgress)
        {
            await _backendDbContext.RoadmapProgresses.AddAsync(roadmapProgress);
            await _backendDbContext.SaveChangesAsync();
        }
    }
}
