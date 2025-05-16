using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IEClassroomService
    {
        Task<ApiResponseDTO<List<Class>>> GetStudentClasses(int studentId);
        Task<ApiResponseDTO<Class>> GetClassById(int classId);
        Task<List<Class>> GetTeacherClasses(int teacherId);
        Task<List<Student>> GetClassStudents(int classId);
    }
}
