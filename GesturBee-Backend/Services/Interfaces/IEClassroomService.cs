using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IEClassroomService
    {
        Task<ApiResponseDTO<List<Class>>> GetStudentClasses(int studentId);
        Task<ApiResponseDTO<Class>> GetClassById(int classId);
        Task<ApiResponseDTO<List<Class>>> GetTeacherClasses(int teacherId);
        Task<ApiResponseDTO<List<Student>>> GetClassStudents(int classId);
        Task<ApiResponseDTO<object>> AddStudentToClass(int studentId, int classId);
        Task<ApiResponseDTO<object>> InviteStudentToClass(int studentId, int classId);
        Task<ApiResponseDTO<object>> CreateClass(CreateClassDTO info);
    }
}
