using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;

namespace GesturBee_Backend.Services
{
    public class EClassroomService : IEClassroomService
    {
        private readonly IEClassroomRepository _eClassroomRepository;

        public EClassroomService(IEClassroomRepository eClassroomRepository)
        {
            _eClassroomRepository = eClassroomRepository;
        }

    }
}
