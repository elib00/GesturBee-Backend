using Microsoft.EntityFrameworkCore;

using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Models;

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

    }
}
