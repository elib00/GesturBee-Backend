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
        Task<ApiResponseDTO> AddStudentToClass(int studentId, int classId);
        Task<ApiResponseDTO> CreateClass(CreateClassDTO info);
        Task<ApiResponseDTO> RequestClassEnrollment(int studentId, int classId);
        Task<ApiResponseDTO> ProcessEnrollmentRequest(ClassAdmissionDTO classAdmissionDetails);
        Task<ApiResponseDTO> RemoveStudentFromClass(int studentId, int classId);
        Task<ApiResponseDTO<ICollection<User>>> GetClassEnrollmentRequests(int classId);
        Task<ApiResponseDTO<List<User>>> GetAllUsersNotEnrolledInClass(int classId);
        Task<ApiResponseDTO<GetExerciseDTO>> GetExerciseById(int exerciseId);
        Task<ApiResponseDTO> CreateExercise(CreateExerciseDTO info);
        Task<ApiResponseDTO> EditExerciseItem(EditExerciseItemDTO exerciseItem);
        Task<ApiResponseDTO<List<Exercise>>> GetTeacherExercises(int teacherId);
        Task<ApiResponseDTO> CreateBatchExerciseContent(List<CreateExerciseContentDTO> exerciseContents);
        Task<ApiResponseDTO<ContentS3KeyDTO>> GetContentS3Key(GetContentS3KeyDTO info);
        Task<ApiResponseDTO> CreateBatchExerciseItemAnswer(int exerciseId, List<ExerciseItemAnswerDTO> exerciseItemAnswers);
        Task<ApiResponseDTO<List<ExerciseItemAnswerDTO>>> GetBatchExerciseItemAnswer(int exerciseId);
    }

}
