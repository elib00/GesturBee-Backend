using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;

namespace GesturBee_Backend.Repository
{
    public class RoadmapRepository : IRoadmapRepository
    {
        private readonly BackendDbContext _backendDbContext;

        public RoadmapRepository(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

        public async Task<Level> GetLevelById(int levelId)
        {
            return await _backendDbContext.Levels.FindAsync(levelId);
        }

        public async Task MarkLevelAsCompleted(Level level)
        {
            level.IsCompleted = true;
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task CreateExercise(CreateExerciseDTO exercise)
        {
            Exercise newExercise = new Exercise
            {
                TeacherId = exercise.TeacherId,
                ExerciseTitle = exercise.ExerciseTitle,
                ExerciseDescription = exercise.ExerciseDescription,
                CreatedAt = DateTime.UtcNow,
                ExerciseItems = exercise.ExerciseItems
            };

            await _backendDbContext.Exercises.AddAsync(newExercise);
            await _backendDbContext.SaveChangesAsync();
        }
    }
}
