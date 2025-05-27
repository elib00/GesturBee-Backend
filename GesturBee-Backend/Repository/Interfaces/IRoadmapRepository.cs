using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IRoadmapRepository
    {
        Task<RoadmapProgress> GetRoadmapProgressWithUserId(int userId);
        Task EditRoadmapProgress(RoadmapProgress roadmapProgress, RoadmapProgressDTO newProgress);
    }
}
