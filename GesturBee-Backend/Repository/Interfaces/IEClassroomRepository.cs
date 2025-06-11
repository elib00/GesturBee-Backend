using GesturBee_Backend.DTO;
using GesturBee_Backend.Models;
using System.Collections.Generic;

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
        Task<Exercise?> GetExerciseById(int exerciseId);
        Task<Exercise> CreateExercise(CreateExerciseDTO info);
        Task EditExerciseItem(EditExerciseItemDTO exerciseItem);
        Task<List<Exercise>> GetTeacherExercises(int teacherId);
        Task CreateBatchExerciseContent(List<CreateExerciseContentDTO> exerciseContents);
        //Task UpdateExerciseIdOfExerciseContents(int exerciseId, string batchId);
        //Task<List<ExerciseContent>> GetAllExerciseContents(string batchId);
        Task<ContentS3KeyDTO> GetContentS3Key(string batchId, int itemNumber);
        Task<List<ExerciseContent>> GetExerciseContents(string batchId);
        Task CreateBatchExerciseItemAnswer(int classExerciseId, int userId, List<ExerciseItemAnswerDTO> exerciseItemAnswers);
        Task<List<ExerciseItemAnswerDTO>> GetBatchExerciseItemAnswer(int classExerciseId, int userId);
        Task CreateClassExercise(int classId, int exerciseId);
        Task<ClassExercise> CheckIfClassExerciseAlreadyExists(int classId, int exerciseId);
        Task<List<ClassExerciseDTO>> GetClassExercises(int classId);
        Task<List<GetUnassignedExerciseDTO>> GetUnassignedClassExercises(int classId, int teacherId);
    }
}
