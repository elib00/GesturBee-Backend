using Microsoft.EntityFrameworkCore;

using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Models;
using MimeKit.Tnef;
using GesturBee_Backend.DTO;
using System.Threading.Tasks;
using GesturBee_Backend.Helpers;

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

        public async Task<EnrollmentRequest> GetEnrollmentRequest(int studentId, int classId)
        {
            return await _backendDbContext.EnrollmentRequests
                .FirstOrDefaultAsync(enrollmentRequest => enrollmentRequest.StudentId == studentId && enrollmentRequest.ClassId == classId);
        }

        public async Task<ICollection<User>> GetClassEnrollmentRequests(int classId)
        {
            return await _backendDbContext.EnrollmentRequests
                .Include(er => er.Student)
                    .ThenInclude(s => s.Profile)
                .Where(er => er.ClassId == classId)
                .Select(er => er.Student)
                .ToListAsync();
        }

        public async Task RemoveStudentClass(StudentClass studentClass)
        {
            _backendDbContext.StudentClasses.Remove(studentClass);
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsersNotEnrolledInClass(int classId)
        {
            IQueryable<int> enrolledUserIds = _backendDbContext.StudentClasses
               .Where(sc => sc.ClassId == classId)
               .Select(sc => sc.StudentId);

            return await _backendDbContext.Users
                .Include(u => u.Profile)
                .Where(u => !enrolledUserIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<Exercise?> GetExerciseById(int exerciseId)
        {
            return await _backendDbContext.Exercises
                .Include(exercise => exercise.ExerciseItems)
                .FirstOrDefaultAsync(exercise => exercise.Id == exerciseId);
        }

        public async Task<List<Exercise>> GetTeacherExercises(int teacherId)
        {
            return await _backendDbContext.Exercises
                .Include(exercise => exercise.ExerciseItems)
                .Where(exercise => exercise.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<Exercise> CreateExercise(CreateExerciseDTO exercise)
        {
            Exercise newExercise = new()
            {
                TeacherId = exercise.TeacherId,
                ExerciseTitle = exercise.ExerciseTitle,
                ExerciseDescription = exercise.ExerciseDescription,
                BatchId = exercise.BatchId,
                CreatedAt = DateTime.UtcNow,
            };

            await _backendDbContext.Exercises.AddAsync(newExercise);
            await _backendDbContext.SaveChangesAsync();

            List<ExerciseItem> exerciseItems = [];

            if (exercise.Type == "Base")
            {
                exerciseItems = exercise.ExerciseItems.Select(item => new ExerciseItem
                {
                    ItemNumber = item.ItemNumber,
                    Question = item.Question,
                    CorrectAnswer = item.CorrectAnswer,
                    Exercise = newExercise
                }).ToList();
            }
            else if (exercise.Type == "MultipleChoice")
            {
                foreach (var item in exercise.ExerciseItems)
                {
                    exerciseItems.Add(new MultipleChoiceItem
                    {
                        ItemNumber = item.ItemNumber,
                        Question = item.Question,
                        CorrectAnswer = item.CorrectAnswer,
                        Exercise = newExercise,
                        ChoiceA = item.ChoiceA,
                        ChoiceB = item.ChoiceB,
                        ChoiceC = item.ChoiceC,
                        ChoiceD = item.ChoiceD
                    });
                }

            }

            await _backendDbContext.ExerciseItems.AddRangeAsync(exerciseItems);
            await _backendDbContext.SaveChangesAsync();

            // Optional: attach items if needed
            newExercise.ExerciseItems = exerciseItems;

            return newExercise;
        }

        public async Task EditExerciseItem(EditExerciseItemDTO exerciseItem)
        {
            int exerciseItemId = exerciseItem.ExerciseItemId;
            ExerciseItem? item = await GetExerciseItemById(exerciseItemId);

            //extension methods
            exerciseItem.ItemNumber.UpdateIfChanged(item.ItemNumber, val => item.ItemNumber = val);
            exerciseItem.Question.UpdateIfChanged(item.Question, val => item.Question = val);
            exerciseItem.CorrectAnswer.UpdateIfChanged(item.CorrectAnswer, val => item.CorrectAnswer = val);

            if (item is MultipleChoiceItem mcItem)
            {
                exerciseItem.ChoiceA.UpdateIfChanged(mcItem.ChoiceA, val => mcItem.ChoiceA = val);
                exerciseItem.ChoiceB.UpdateIfChanged(mcItem.ChoiceB, val => mcItem.ChoiceB = val);
                exerciseItem.ChoiceC.UpdateIfChanged(mcItem.ChoiceC, val => mcItem.ChoiceC = val);
                exerciseItem.ChoiceD.UpdateIfChanged(mcItem.ChoiceD, val => mcItem.ChoiceD = val);
            }

            await _backendDbContext.SaveChangesAsync();
        }


        public async Task CreateBatchExerciseContent(List<CreateExerciseContentDTO> exerciseContents)
        {
            List<ExerciseContent> entities = exerciseContents.Select(exerciseContent => new ExerciseContent
            {
                ContentS3Key = exerciseContent.ContentS3Key,
                ContentType = exerciseContent.ContentType,
                BatchId = exerciseContent.BatchId,
                ItemNumber = exerciseContent.ItemNumber
            }).ToList();

            await _backendDbContext.ExerciseContents.AddRangeAsync(entities);
            await _backendDbContext.SaveChangesAsync();
        }

        public async Task<ContentS3KeyDTO> GetContentS3Key(string batchId, int itemNumber)
        {
            ExerciseContent? content = await _backendDbContext.ExerciseContents
                .FirstOrDefaultAsync(exerciseContent => exerciseContent.ItemNumber == itemNumber && exerciseContent.BatchId == batchId);

            return new ContentS3KeyDTO
            {
                Key = content.ContentS3Key
            };
        }

        private async Task<ExerciseItem?> GetExerciseItemById(int exerciseItemId)
        {
            return await _backendDbContext.ExerciseItems.FindAsync(exerciseItemId);
        }
    }
}
