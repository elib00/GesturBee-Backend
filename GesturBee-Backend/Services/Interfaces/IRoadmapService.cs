﻿using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IRoadmapService
    {
        Task<ApiResponseDTO> EditRoadmapProgress(int roadmapProgressId, RoadmapProgressDTO newProgress);
        Task<ApiResponseDTO<RoadmapProgressDTO>> GetRoadmapProgressWithUserId(int userId);
    }
}
