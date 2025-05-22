using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IRoadmapRepository
    {
        Task<Level> GetLevelById(int levelId);
        Task MarkLevelAsCompleted(Level level);
        Task CreateExercise(CreateExerciseDTO info);
        Task EditExerciseItem(ExerciseItemDTO exerciseItem);
        Task<RoadmapProgress> GetRoadmapProgressWithUserId(int userId);
        Task EditRoadmapProgress(RoadmapProgress roadmapProgress, RoadmapProgressDTO newProgress);
    }
}
