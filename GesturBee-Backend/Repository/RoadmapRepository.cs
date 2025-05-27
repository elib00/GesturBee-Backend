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

        public async Task EditRoadmapProgress(RoadmapProgress roadmapProgress, RoadmapProgressDTO newProgress)
        {
            roadmapProgress.Level = newProgress.Level;
            roadmapProgress.Stage = newProgress.Stage;
            await _backendDbContext.SaveChangesAsync();
        }


    }
}
