using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;

namespace GesturBee_Backend.Services
{
    public class RoadmapService : IRoadmapService
    {
        private readonly IRoadmapRepository _roadmapRepository;

        public RoadmapService(IRoadmapRepository roadmapRepository)
        {
            _roadmapRepository = roadmapRepository;
        }


    }
}
