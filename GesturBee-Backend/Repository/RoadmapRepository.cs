using GesturBee_Backend.Repository.Interfaces;

namespace GesturBee_Backend.Repository
{
    public class RoadmapRepository : IRoadmapRepository
    {
        private readonly BackendDbContext _backendDbContext;

        public RoadmapRepository(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }
    }
}
