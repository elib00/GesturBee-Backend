﻿using GesturBee_Backend.Models;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> IsExistingEmail(string email);
        Task<User?> GetUserByEmail(string email);
        Task<bool> CreateUser(User user);
        Task ResetPassword(string email, string newPassword);
        Task UpdateLastLogin(User user);
        Task CreateRoadmapProgress(RoadmapProgress roadmapProgress);
    }
}
