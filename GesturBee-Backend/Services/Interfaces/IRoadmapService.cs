using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IRoadmapService
    {
        Task<ApiResponseDTO> MarkLevelAsCompleted(int levelId);
        Task<ApiResponseDTO> CreateExercise(CreateExerciseDTO info);
        Task<ApiResponseDTO> EditExerciseItem(ExerciseItemDTO exerciseItem);
        Task<ApiResponseDTO> EditRoadmapProgress(int roadmapProgressId, RoadmapProgressDTO newProgress);
    }
}
