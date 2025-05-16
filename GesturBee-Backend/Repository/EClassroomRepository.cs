using Microsoft.EntityFrameworkCore;

using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Models;
using MimeKit.Tnef;
using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Repository
{
    public class EClassroomRepository : IEClassroomRepository
    {
        private readonly BackendDbContext _backendDbContext;

        public EClassroomRepository(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

        public async Task<Class> GetClassById(int classId)
        {
            return await _backendDbContext.Classes.FindAsync(classId);
        }

        public async Task<Student> GetStudentById(int studentId)
        {
            return await _backendDbContext.Students.FindAsync(studentId);
        }

        public async Task<Teacher> GetTeacherById(int teacherId)
        {
            return await _backendDbContext.Teachers.FindAsync(teacherId);
        }

        public async Task<List<Class>> GetStudentClasses(int studentId)
        {
            return await _backendDbContext.StudentClasses
                .AsNoTracking()
                .Where(studentClass => studentClass.StudentId == studentId)
                .Include(studentClass => studentClass.Class)
                    .ThenInclude(cls => cls.Teacher)
                        .ThenInclude(teacher => teacher.User)
                            .ThenInclude(user => user.Account)
                .Select(studentClass => studentClass.Class)  // Extract just the Class
                .ToListAsync();
        }

        public async Task<List<Class>> GetTeacherClasses(int teacherId)
        {
            return await _backendDbContext.Classes
                .AsNoTracking()
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.Teacher)
                    .ThenInclude(teacher => teacher.User)
                        .ThenInclude(user => user.Profile)
                .ToListAsync();
        }

        public async Task<List<Student>> GetClassStudents(int classId)
        {
            return await _backendDbContext.StudentClasses
                .AsNoTracking()
                .Where(studentClass => studentClass.ClassId == classId)
                .Include(studentClass => studentClass.Student)
                    .ThenInclude(student => student.User)
                        .ThenInclude(user => user.Profile)
                .Select(studentClass => studentClass.Student)
                .ToListAsync();
        }

        public async Task AddStudentToClass(Student student, Class cls)
        {
            await _backendDbContext.StudentClasses.AddAsync(new StudentClass
            {
                Student = student,
                Class = cls
            });

            await _backendDbContext.SaveChangesAsync();
        }

        public async Task InviteStudentToClass(Student student, Class cls)
        {
            await _backendDbContext.ClassInvitations.AddAsync(new ClassInvitation
            {
                Student = student,
                Class = cls,
                InvitedAt = DateTime.UtcNow
            });

            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<bool> StudentAlreadyInvited(int studentId, int classId)
        {
            return await _backendDbContext.ClassInvitations
                .AnyAsync(classInvitation => classInvitation.StudentId == studentId && classInvitation.ClassId == classId);
        }

        public async Task CreateClass(CreateClassDTO info)
        {
            Class cls = new Class
            {
                TeacherId = info.TeacherId,
                ClassName = info.ClassName,
                ClassDescription = info.ClassDescription,
                CreatedAt = DateTime.UtcNow,
            };

            await _backendDbContext.Classes.AddAsync(cls);
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<bool> ClassNameAlreadyTaken(string className)
        {
            return await _backendDbContext.Classes.AnyAsync(c => c.ClassName == className);
        }

        public async Task RequestClassEnrollment(Student student, Class cls)
        {
            await _backendDbContext.EnrollmentRequests.AddAsync(new EnrollmentRequest
            {
                Student = student,
                Class = cls,
                RequestedAt = DateTime.UtcNow
            });

            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<bool> RequestForClassEnrollmentAlreadySent(int studentId, int classId)
        {
            return await _backendDbContext.EnrollmentRequests
                .AnyAsync(enrollmentRequest => enrollmentRequest.StudentId == studentId && enrollmentRequest.ClassId == classId);
        }
    }
}
