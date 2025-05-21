using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Helpers;

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

        public async Task<ExerciseItem> GetExerciseItemById(int exerciseItemId)
        {
            return await _backendDbContext.ExerciseItems.FindAsync(exerciseItemId);
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

        public async Task EditExerciseItem(ExerciseItemDTO exerciseItem)
        {
            int exerciseItemId = exerciseItem.ExerciseItemId;
            ExerciseItem item = await GetExerciseItemById(exerciseItemId);

            //extension methods
            exerciseItem.ItemNumber.UpdateIfChanged(item.ItemNumber, val => item.ItemNumber = val);
            exerciseItem.Question.UpdateIfChanged(item.Question, val => item.Question = val);
            exerciseItem.ChoiceA.UpdateIfChanged(item.ChoiceA, val => item.ChoiceA = val);
            exerciseItem.ChoiceB.UpdateIfChanged(item.ChoiceB, val => item.ChoiceB = val);
            exerciseItem.ChoiceC.UpdateIfChanged(item.ChoiceC, val => item.ChoiceC = val);
            exerciseItem.ChoiceD.UpdateIfChanged(item.ChoiceD, val => item.ChoiceD = val);
            exerciseItem.CorrectAnswer.UpdateIfChanged(item.CorrectAnswer, val => item.CorrectAnswer = val);

            await _backendDbContext.SaveChangesAsync();
        }
    }
}
