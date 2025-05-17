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
        Task<ApiResponseDTO<object>> AddStudentToClass(StudentAndClassDTO info);
        Task<ApiResponseDTO<object>> InviteStudentToClass(StudentAndClassDTO info);
        Task<ApiResponseDTO<object>> CreateClass(CreateClassDTO info);
        Task<ApiResponseDTO<object>> RequestClassEnrollment(StudentAndClassDTO info);
        Task<ApiResponseDTO<object>> ProcessEnrollmentRequest(ClassAdmissionDTO classAdmissionDetails);
        Task<ApiResponseDTO<object>> ProcessInvitationRequest(ClassAdmissionDTO classAdmissionDetails);
        Task<ApiResponseDTO<List<ClassEnrollmentGroupDTO>>> GetTeacherClassEnrollmentRequests(int teacherId);
        Task<ApiResponseDTO<List<ClassInvitationGroupDTO>>> GetStudentClassInvitationRequests(int studentId);
        Task<ApiResponseDTO<object>> RemoveStudentFromClass(StudentAndClassDTO info);
    }

}
