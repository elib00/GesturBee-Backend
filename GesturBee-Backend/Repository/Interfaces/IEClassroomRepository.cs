using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using MimeKit.Tnef;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IEClassroomRepository
    {
        Task<List<Class>> GetStudentClasses(int  studentId);
        Task<Class> GetClassById(int classId);
        Task<User> GetUserById(int studentId);
        Task<List<Class>> GetTeacherClasses(int teacherId);
        Task<List<User>> GetClassStudents(int classId);
        Task AddStudentToClass(int studentId, int classId);
        Task<ICollection<User>> GetClassEnrollmentRequests(int classId);
        Task CreateClass(CreateClassDTO info);
        Task<bool> ClassNameAlreadyTaken(string className);
        Task RequestClassEnrollment(int studentId, int classId);
        Task<bool> RequestForClassEnrollmentAlreadySent(int studentId, int classId);
        Task AcceptEnrollmentRequest(EnrollmentRequest enrollmentRequest);
        Task RejectEnrollmentRequest(EnrollmentRequest enrollmentRequest);
        Task<EnrollmentRequest> GetEnrollmentRequest(int studentId, int classId);
        //Task<List<ClassEnrollmentGroupDTO>> GetTeacherClassEnrollmentRequests(int teacherId);
        Task RemoveStudentClass(StudentClass studentClass);
        Task<StudentClass> GetStudentClass(int studentId, int classId);
        Task<List<User>> GetAllUsersNotEnrolledInClass(int classId);
    }
}
