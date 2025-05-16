using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;

namespace GesturBee_Backend.Repository.Interfaces
{
    public interface IEClassroomRepository
    {
        Task<List<Class>> GetStudentClasses(int  studentId);
        Task<Class> GetClassById(int classId);
        Task<Student> GetStudentById(int studentId);
        Task<List<Class>> GetTeacherClasses(int teacherId);
        Task<List<Student>> GetClassStudents(int classId);
        Task AddStudentToClass(Student student, Class cls);
        Task InviteStudentToClass(Student student, Class cls);
        Task<bool> StudentAlreadyInvited(int studentId, int classId);
        Task CreateClass(CreateClassDTO info);
        Task<Teacher> GetTeacherById(int teacherId);
        Task<bool> ClassNameAlreadyTaken(string className);
    }
}
