﻿using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Repository;

namespace GesturBee_Backend.Services
{
    public class RoadmapService : IRoadmapService
    {
        private readonly IRoadmapRepository _roadmapRepository;

        public RoadmapService(IRoadmapRepository roadmapRepository)
        {
            _roadmapRepository = roadmapRepository;
        }

        public async Task<ApiResponseDTO> EditRoadmapProgress(int userId, RoadmapProgressDTO newProgress)
        {
            RoadmapProgress roadmapProgress = await _roadmapRepository.GetRoadmapProgressWithUserId(userId);
            if (roadmapProgress == null)
            {
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.RoadmapProgressNotFound
                };
            }

            await _roadmapRepository.EditRoadmapProgress(roadmapProgress, newProgress);

            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.RoadmapProgressEditSuccessful
            };
        }

        public async Task<ApiResponseDTO<RoadmapProgressDTO>>GetRoadmapProgressWithUserId(int userId)
        {
            RoadmapProgress roadmapProgress = await _roadmapRepository.GetRoadmapProgressWithUserId(userId);
            if(roadmapProgress == null)
            {
                return new ApiResponseDTO<RoadmapProgressDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.RoadmapProgressNotFound
                };
            }

            return new ApiResponseDTO<RoadmapProgressDTO>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = new RoadmapProgressDTO
                {
                    Stage = roadmapProgress.Stage,
                    Level = roadmapProgress.Level,
                }
            };
        }
    }
}
