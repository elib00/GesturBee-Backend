using Microsoft.EntityFrameworkCore;

using GesturBee_Backend.Repository.Interfaces;

namespace GesturBee_Backend.Repository
{
    public class EClassroomRepository : IEClassroomRepository
    {
        private readonly BackendDbContext _backendDbContext;

        public EClassroomRepository(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

    }
}
