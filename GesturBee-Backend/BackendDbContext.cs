using Microsoft.EntityFrameworkCore;
using GesturBee_Backend.Models;

namespace GesturBee_Backend
{
    public class BackendDbContext : DbContext
    {
        public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<ClassInvitation> ClassInvitations { get; set; }
        public DbSet<EnrollmentRequest> EnrollmentRequests { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseItem> ExerciseItems { get; set; }
    }
}
