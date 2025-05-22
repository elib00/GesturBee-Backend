using Microsoft.EntityFrameworkCore;

using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Models;
using MimeKit.Tnef;
using GesturBee_Backend.DTO;
using System.Threading.Tasks;

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

        public async Task<User> GetUserById(int studentId)
        {
            return await _backendDbContext.Users.FindAsync(studentId);
        }

        public async Task<StudentClass> GetStudentClass(int studentId, int classId)
        {
            return await _backendDbContext.StudentClasses
                .FirstOrDefaultAsync(studentClass => studentClass.StudentId == studentId && studentClass.ClassId == classId);

        }

        public async Task<List<Class>> GetStudentClasses(int studentId)
        {
            return await _backendDbContext.StudentClasses
                .AsNoTracking()
                .Where(studentClass => studentClass.StudentId == studentId)
                .Include(studentClass => studentClass.Class)
                    .ThenInclude(cls => cls.Teacher)
                        .ThenInclude(teacher => teacher.Profile)
                .Select(studentClass => studentClass.Class) //project just the class
                .ToListAsync();
        }

        public async Task<List<Class>> GetTeacherClasses(int teacherId)
        {
            return await _backendDbContext.Classes
                .AsNoTracking()
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }


        public async Task<List<User>> GetClassStudents(int classId)
        {
            return await _backendDbContext.StudentClasses
                .AsNoTracking()
                .Where(studentClass => studentClass.ClassId == classId)
                .Include(studentClass => studentClass.Student)
                    .ThenInclude(user => user.Profile)
                .Select(studentClass => studentClass.Student)
                .ToListAsync();
        }

        public async Task AddStudentToClass(int studentId, int classId)
        {
            await _backendDbContext.StudentClasses.AddAsync(new StudentClass
            {
                StudentId = studentId,
                ClassId = classId
            });

            await _backendDbContext.SaveChangesAsync();
        }

        public async Task InviteStudentToClass(int studentId, int classId)
        {
            await _backendDbContext.ClassInvitations.AddAsync(new ClassInvitation
            {
                StudentId = studentId,
                ClassId = classId,
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
            //create the class code
            string guid = Guid.NewGuid().ToString("N").ToUpper();
            string shortCode = guid.Substring(0, 4) + guid[^3..]; ; //shorten GUID -> first 4, last 3
            string classCode = $"GB-{shortCode}";

            Class cls = new Class
            {
                TeacherId = info.TeacherId,
                ClassName = info.ClassName,
                ClassDescription = info.ClassDescription,
                ClassCode = classCode,
                CreatedAt = DateTime.UtcNow,
            };

            await _backendDbContext.Classes.AddAsync(cls);
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<bool> ClassNameAlreadyTaken(string className)
        {
            return await _backendDbContext.Classes.AnyAsync(c => c.ClassName == className);
        }

        public async Task RequestClassEnrollment(int studentId, int classId)
        {
            await _backendDbContext.EnrollmentRequests.AddAsync(new EnrollmentRequest
            {
                StudentId = studentId,
                ClassId = classId,
                RequestedAt = DateTime.UtcNow
            });

            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<bool> RequestForClassEnrollmentAlreadySent(int studentId, int classId)
        {
            return await _backendDbContext.EnrollmentRequests
                .AnyAsync(enrollmentRequest => enrollmentRequest.StudentId == studentId && enrollmentRequest.ClassId == classId);
        }

        private async Task RemoveEnrollmentRequest(EnrollmentRequest enrollmentRequest)
        {
            _backendDbContext.EnrollmentRequests.Remove(enrollmentRequest);
            await _backendDbContext.SaveChangesAsync();
        }

        private async Task RemoveClassInvitation(ClassInvitation classInvitation)
        {
            _backendDbContext.ClassInvitations.Remove(classInvitation)  ;
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task AcceptEnrollmentRequest(EnrollmentRequest enrollmentRequest)
        {
            User student = enrollmentRequest.Student;
            Class cls = enrollmentRequest.Class;

            //add the student to the class
            StudentClass studentClass = new StudentClass
            {
                StudentId = student.Id,
                ClassId = cls.Id
            };

            await _backendDbContext.StudentClasses.AddAsync(studentClass);
            await RemoveEnrollmentRequest(enrollmentRequest);
        }

        public async Task RejectEnrollmentRequest(EnrollmentRequest enrollmentRequest)
        {
            await RemoveEnrollmentRequest(enrollmentRequest);
        }

        public async Task AcceptInvitationRequest(ClassInvitation invitation)
        {
            User student = invitation.Student;
            Class cls = invitation.Class;

            //add the student to the class
            StudentClass studentClass = new StudentClass
            {
                StudentId = student.Id,
                ClassId = cls.Id
            };

            await _backendDbContext.StudentClasses.AddAsync(studentClass);
            await RemoveClassInvitation(invitation);
        }

        public async Task RejectInvitationRequest(ClassInvitation invitation)
        {
            await RemoveClassInvitation(invitation);
        }

        public async Task<EnrollmentRequest> GetEnrollmentRequest(int studentId, int classId)
        {
            return await _backendDbContext.EnrollmentRequests
                .FirstOrDefaultAsync(enrollmentRequest => enrollmentRequest.StudentId == studentId && enrollmentRequest.ClassId == classId);
        }

        public async Task<ClassInvitation> GetClassInvitation(int studentId, int classId)
        {
            return await _backendDbContext.ClassInvitations
               .FirstOrDefaultAsync(enrollmentRequest => enrollmentRequest.StudentId == studentId && enrollmentRequest.ClassId == classId);
        }

        public async Task<List<ClassEnrollmentGroupDTO>> GetTeacherClassEnrollmentRequests(int teacherId)
        {
            return await _backendDbContext.EnrollmentRequests
                .Include(er => er.Student) //eager load
                        .ThenInclude(u => u.Profile)
               .Where(er => er.Class.TeacherId == teacherId)
               .GroupBy(er => new { er.ClassId, er.Class.ClassName })
               .Select(g => new ClassEnrollmentGroupDTO
               {
                   ClassId = g.Key.ClassId,
                   ClassName = g.Key.ClassName,
                   Requests = g.Select(er => new EnrollmentRequestDTO
                   {
                       StudentName = er.Student.Profile.FirstName + " " + er.Student.Profile.LastName,
                       RequestedAt = er.RequestedAt
                   }).ToList()
               })
               .ToListAsync();
        }

        public async Task<List<ClassInvitationGroupDTO>> GetStudentClassInvitationRequests(int studentId)
        {
            return await _backendDbContext.ClassInvitations
                .Include(ci => ci.Class) //eager load
                    .ThenInclude(cls => cls.Teacher)
                        .ThenInclude(user => user.Profile)
               .Where(ci => ci.StudentId == studentId)
               .GroupBy(ci => new { ci.ClassId, ci.Class.ClassName })
               .Select(g => new ClassInvitationGroupDTO
               {
                   ClassId = g.Key.ClassId,
                   ClassName = g.Key.ClassName,
                   Requests = g.Select(ci => new InvitationRequestDTO
                   {
                       TeacherName = ci.Class.Teacher.Profile.FirstName + " " + ci.Class.Teacher.Profile.LastName,
                       InvitedAt = ci.InvitedAt
                   }).ToList()
               })
               .ToListAsync();
        }

        public async Task RemoveStudentClass(StudentClass studentClass)
        {
            _backendDbContext.StudentClasses.Remove(studentClass);
            await _backendDbContext.SaveChangesAsync();
        }
    }
}
