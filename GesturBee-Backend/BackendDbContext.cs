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
        public DbSet<Class> Classes { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<ClassInvitation> ClassInvitations { get; set; }
        public DbSet<EnrollmentRequest> EnrollmentRequests { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseItem> ExerciseItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(u => u.TaughtClasses)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<ClassInvitation>(classInvitation =>
            {
                classInvitation.HasKey(ci => ci.Id);

                classInvitation.HasOne(ci => ci.Class)
                    .WithMany(c => c.ClassInvitations)
                    .HasForeignKey(ci => ci.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);  // keep this if it's safe to cascade on Class deletion

                classInvitation.HasOne(ci => ci.Student)
                    .WithMany(s => s.ClassInvitations)
                    .HasForeignKey(ci => ci.StudentId)
                    .OnDelete(DeleteBehavior.Restrict); // prevents cascade path conflict
            });

            modelBuilder.Entity<EnrollmentRequest>(enrollmentRequest =>
            {
                enrollmentRequest.HasKey(er => er.Id);

                enrollmentRequest.HasOne(er => er.Class)
                    .WithMany(c => c.EnrollmentRequests)
                    .HasForeignKey(ci => ci.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);

                enrollmentRequest.HasOne(ci => ci.Student)
                    .WithMany(s => s.EnrollmentRequests)
                    .HasForeignKey(ci => ci.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StudentClass>(studentClass =>
            {
                studentClass.HasKey(sc => sc.Id);

                studentClass.HasOne(sc => sc.Class)
                    .WithMany(c => c.StudentClasses)
                    .HasForeignKey(sc => sc.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);

                studentClass.HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentClasses)
                    .HasForeignKey(sc => sc.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }



    }
}
