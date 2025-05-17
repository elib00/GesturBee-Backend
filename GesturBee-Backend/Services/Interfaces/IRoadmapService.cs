using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IRoadmapService
    {
        Task<ApiResponseDTO<object>> MarkLevelAsCompleted(int levelId);
        Task<ApiResponseDTO<object>> CreateExercise(CreateExerciseDTO info);
    }
}
