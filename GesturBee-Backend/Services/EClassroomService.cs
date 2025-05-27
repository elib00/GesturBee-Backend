using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;

namespace GesturBee_Backend.Services
{
    public class EClassroomService : IEClassroomService
    {
        private readonly IEClassroomRepository _eClassroomRepository;

        public EClassroomService(IEClassroomRepository eClassroomRepository)
        {
            _eClassroomRepository = eClassroomRepository;
        }

        public async Task<ApiResponseDTO<List<Class>>> GetStudentClasses(int studentId)
        {
            List<Class> studentClasses = await _eClassroomRepository.GetStudentClasses(studentId);
            return new ApiResponseDTO<List<Class>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = studentClasses
            };
        }

        public async Task<ApiResponseDTO<Class>> GetClassById(int classId)
        {
            Class cls = await _eClassroomRepository.GetClassById(classId);
            return new ApiResponseDTO<Class>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = cls
            };
        }

        public async Task<ApiResponseDTO<List<Class>>> GetTeacherClasses(int teacherId)
        {
            List<Class> teacherClasses = await _eClassroomRepository.GetTeacherClasses(teacherId);
            return new ApiResponseDTO<List<Class>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = teacherClasses
            };
        }

        public async Task<ApiResponseDTO<List<User>>> GetClassStudents(int classId)
        {
            List<User> students = await _eClassroomRepository.GetClassStudents(classId);
            return new ApiResponseDTO<List<User>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = students
            };
        }

        public async Task<ApiResponseDTO> AddStudentToClass(int studentId, int classId)
        {
            ApiResponseDTO checkStudentAndClass = await CheckStudentAndClassIfNull(studentId, classId);
            if (!checkStudentAndClass.Success) return checkStudentAndClass;

            await _eClassroomRepository.AddStudentToClass(studentId, classId);

            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.StudentAddedToClassroom
            };
        }

        public async Task<ApiResponseDTO> CreateClass(CreateClassDTO info)
        {
            int teacherId = info.TeacherId;
            User teacher = await _eClassroomRepository.GetUserById(teacherId);

            if (teacher == null)
            {
                return new ApiResponseDTO { Success = false, ResponseType = ResponseType.TeacherNotFound };
            }

            bool isNameAlreadyTaken = await _eClassroomRepository.ClassNameAlreadyTaken(info.ClassName);

            if (isNameAlreadyTaken)
            {
                return new ApiResponseDTO { Success = false, ResponseType = ResponseType.ClassNameAlreadyTaken };
            }


            await _eClassroomRepository.CreateClass(info);

            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.ClassCreated
            };
        }

        public async Task<ApiResponseDTO> RequestClassEnrollment(int studentId, int classId)
        {
            ApiResponseDTO checkStudentAndClass = await CheckStudentAndClassIfNull(studentId, classId);
            if (!checkStudentAndClass.Success) return checkStudentAndClass;

            bool hasAlreadyRequested = await _eClassroomRepository.RequestForClassEnrollmentAlreadySent(studentId, classId);

            if (hasAlreadyRequested)
            {
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.EnrollmentRequestAlreadySent
                };
            }

            await _eClassroomRepository.RequestClassEnrollment(studentId, classId);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.EnrollmentRequestSuccessful
            };
        }

        public async Task<ApiResponseDTO> ProcessEnrollmentRequest(ClassAdmissionDTO classAdmissionDetails)
        {
            int studentId = (int) classAdmissionDetails.StudentId;
            int classId = (int) classAdmissionDetails.ClassId;

            ApiResponseDTO checkStudentAndClass = await CheckStudentAndClassIfNull(studentId, classId);

            if (!checkStudentAndClass.Success) return checkStudentAndClass;

            EnrollmentRequest enrollmentRequest = await _eClassroomRepository.GetEnrollmentRequest(studentId, classId);


            if (classAdmissionDetails.Accept)
            {
                await _eClassroomRepository.AcceptEnrollmentRequest(enrollmentRequest);
                return new ApiResponseDTO
                {
                    Success = true,
                    ResponseType = ResponseType.EnrollmentAcceptanceSuccessful
                };
            }
            else
            {
                await _eClassroomRepository.RejectEnrollmentRequest(enrollmentRequest);
                return new ApiResponseDTO
                {
                    Success = true,
                    ResponseType = ResponseType.EnrollmentRejectionSuccessful
                };
            }

        }

        public async Task<ApiResponseDTO> RemoveStudentFromClass(int studentId, int classId)
        {
            ApiResponseDTO checkStudentAndClass = await CheckStudentAndClassIfNull(studentId, classId);
            if (!checkStudentAndClass.Success) return checkStudentAndClass;

            StudentClass studentClassToRemove = await _eClassroomRepository.GetStudentClass(studentId, classId);
            
            await _eClassroomRepository.RemoveStudentClass(studentClassToRemove);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.StudentRemovalSuccessful
            };
        }

        public async Task<ApiResponseDTO<ICollection<User>>> GetClassEnrollmentRequests(int classId)
        {
            Class c = await _eClassroomRepository.GetClassById(classId);

            if (c == null)
            {
                return new ApiResponseDTO<ICollection<User>> { Success = false, ResponseType = ResponseType.ClassNotFound };
            }

            ICollection<User> enrollmentRequests = await _eClassroomRepository.GetClassEnrollmentRequests(classId);
            return new ApiResponseDTO<ICollection<User>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = enrollmentRequests
            };
        }

        //TODO: replace by CheckIfUserExists
        private async Task<ApiResponseDTO> CheckStudentAndClassIfNull(int studentId, int classId)
        {

            User student = await _eClassroomRepository.GetUserById(studentId);

            if (student == null)
            {
                return new ApiResponseDTO { Success = false, ResponseType = ResponseType.StudentNotFound };
            }


            Class cls = await _eClassroomRepository.GetClassById(classId);

            if (cls == null)
            { 
                return new ApiResponseDTO { Success = false, ResponseType = ResponseType.ClassNotFound };
            }

            return new ApiResponseDTO
            {
                Success = true, 
                ResponseType = ResponseType.NoNullValues
            };
        }

        public async Task<ApiResponseDTO<List<User>>> GetAllUsersNotEnrolledInClass(int classId)
        {
            Class cls = await _eClassroomRepository.GetClassById(classId);
            if (cls == null)
            {
                return new ApiResponseDTO<List<User>>
                {
                    Success = false,
                    ResponseType = ResponseType.ClassNotFound
                };
            }

            List<User> users = await _eClassroomRepository.GetAllUsersNotEnrolledInClass(classId);
            return new ApiResponseDTO<List<User>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = users
            };
        }


        public async Task<ApiResponseDTO<Exercise>> GetExerciseById(int exerciseId)
        {
            Exercise? exercise = await _eClassroomRepository.GetExerciseById(exerciseId);

            if (exercise == null)
            {
                return new ApiResponseDTO<Exercise>
                {
                    Success = false,
                    ResponseType = ResponseType.ExerciseNotFound
                };
            }

            return new ApiResponseDTO<Exercise>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = exercise
            };
        }

        public async Task<ApiResponseDTO> CreateExercise(CreateExerciseDTO info)
        {
            await _eClassroomRepository.CreateExercise(info);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.ExerciseCreationSuccessful
            };
        }

        public async Task<ApiResponseDTO> EditExerciseItem(ExerciseItemDTO exerciseItem)
        {
            await _eClassroomRepository.EditExerciseItem(exerciseItem);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.ExerciseItemEditSuccessful
            };
        }

    }
}
