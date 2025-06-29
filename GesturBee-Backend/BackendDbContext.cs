﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<EnrollmentRequest> EnrollmentRequests { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseItem> ExerciseItems { get; set; }
        public DbSet<RoadmapProgress> RoadmapProgresses { get; set; }
        public DbSet<ExerciseContent> ExerciseContents { get; set; }
        public DbSet<ClassExerciseItemAnswer> ExerciseItemAnswers { get; set; }
        public DbSet<ClassExercise> ClassExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<ExerciseItem>()
                .HasDiscriminator<string>("ItemType") // This is the discriminator column
                .HasValue<ExerciseItem>("Base")
                .HasValue<MultipleChoiceItem>("MultipleChoice");

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(u => u.TaughtClasses)
                .HasForeignKey(c => c.TeacherId);

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

            modelBuilder.Entity<ClassExercise>()
                .HasOne(ce => ce.Class)
                .WithMany()
                .HasForeignKey(ce => ce.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassExercise>()
                .HasOne(ce => ce.Exercise)
                .WithMany()
                .HasForeignKey(ce => ce.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
