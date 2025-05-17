using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;

namespace GesturBee_Backend.Services
{
    public class RoadmapService : IRoadmapService
    {
        private readonly IRoadmapRepository _roadmapRepository;

        public RoadmapService(IRoadmapRepository roadmapRepository)
        {
            _roadmapRepository = roadmapRepository;
        }

        public async Task<ApiResponseDTO<object>> MarkLevelAsCompleted(int levelId)
        {
            Level level = await _roadmapRepository.GetLevelById(levelId);

            if(level == null)
            {
                return new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.LevelNotFound
                };
            }

            await _roadmapRepository.MarkLevelAsCompleted(level);

            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.LevelCompleted
            };
        }

        public async Task<ApiResponseDTO<object>> CreateExercise(CreateExerciseDTO info)
        {
            await _roadmapRepository.CreateExercise(info);
            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.ExerciseCreationSuccessful
            };
        }

        public async Task<ApiResponseDTO<object>> EditExerciseItem(ExerciseItemDTO exerciseItem)
        {
            await _roadmapRepository.EditExerciseItem(exerciseItem);
            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.ExerciseItemEditSuccessful
            };
        }

    }
}
