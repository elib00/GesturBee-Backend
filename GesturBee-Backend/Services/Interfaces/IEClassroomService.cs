using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IEClassroomService
    {
        Task<ApiResponseDTO<List<Class>>> GetStudentClasses(int studentId);
        Task<ApiResponseDTO<Class>> GetClassById(int classId);
        Task<ApiResponseDTO<List<Class>>> GetTeacherClasses(int teacherId);
        Task<ApiResponseDTO<List<User>>> GetClassStudents(int classId);
        Task<ApiResponseDTO> AddStudentToClass(StudentAndClassDTO info);
        Task<ApiResponseDTO> InviteStudentToClass(StudentAndClassDTO info);
        Task<ApiResponseDTO> CreateClass(CreateClassDTO info);
        Task<ApiResponseDTO> RequestClassEnrollment(StudentAndClassDTO info);
        Task<ApiResponseDTO> ProcessEnrollmentRequest(ClassAdmissionDTO classAdmissionDetails);
        Task<ApiResponseDTO> ProcessInvitationRequest(ClassAdmissionDTO classAdmissionDetails);
        Task<ApiResponseDTO<List<ClassEnrollmentGroupDTO>>> GetTeacherClassEnrollmentRequests(int teacherId);
        Task<ApiResponseDTO<List<ClassInvitationGroupDTO>>> GetStudentClassInvitationRequests(int studentId);
        Task<ApiResponseDTO> RemoveStudentFromClass(StudentAndClassDTO info);
        Task<ApiResponseDTO<ICollection<User>>> GetClassEnrollmentRequests(int classId);
        //Task<ApiResponseDTO<object>> LeaveClass(StudentAndClassDTO info);
    }

}
