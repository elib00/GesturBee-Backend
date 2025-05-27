using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GesturBee_Backend.Repository
{
    public class RoadmapRepository : IRoadmapRepository
    {
        private readonly BackendDbContext _backendDbContext;

        public RoadmapRepository(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

        public async Task<RoadmapProgress> GetRoadmapProgressWithUserId(int userId)
        {
            return await _backendDbContext.RoadmapProgresses
                .FirstOrDefaultAsync(rp => rp.UserId == userId);
        }

        public async Task<ExerciseItem> GetExerciseItemById(int exerciseItemId)
        {
            return await _backendDbContext.ExerciseItems.FindAsync(exerciseItemId);
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
            exerciseItem.CorrectAnswer.UpdateIfChanged(item.CorrectAnswer, val => item.CorrectAnswer = val);

            if(item is MultipleChoiceItem mcItem)
            {
                exerciseItem.ChoiceA.UpdateIfChanged(mcItem.ChoiceA, val => mcItem.ChoiceA = val);
                exerciseItem.ChoiceB.UpdateIfChanged(mcItem.ChoiceB, val => mcItem.ChoiceB = val);
                exerciseItem.ChoiceC.UpdateIfChanged(mcItem.ChoiceC, val => mcItem.ChoiceC = val);
                exerciseItem.ChoiceD.UpdateIfChanged(mcItem.ChoiceD, val => mcItem.ChoiceD = val);
            }

            await _backendDbContext.SaveChangesAsync();
        }
        public async Task EditRoadmapProgress(RoadmapProgress roadmapProgress, RoadmapProgressDTO newProgress)
        {
            roadmapProgress.Level = newProgress.Level;
            roadmapProgress.Stage = newProgress.Stage;
            await _backendDbContext.SaveChangesAsync();
        }

    }
}
