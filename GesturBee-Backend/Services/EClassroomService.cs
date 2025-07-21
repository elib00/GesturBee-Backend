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
        private readonly IS3Service _s3Service;

        public EClassroomService(IEClassroomRepository eClassroomRepository, IS3Service s3Service)
        {
            _eClassroomRepository = eClassroomRepository;
            _s3Service = s3Service;
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

        public async Task<ApiResponseDTO> RequestClassEnrollment(int studentId, int classId, RequestClassEnrollmentDTO classEnrollmentRequest)
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


            Class classToEnroll = await _eClassroomRepository.GetClassById(classId);

            if(classToEnroll.TeacherId == studentId)
            {
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.EnrollmentForbidden
                };
            }

            bool codeMatch = classEnrollmentRequest.ClassCode == classToEnroll.ClassCode;

            if(!codeMatch)
            {
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.IncorrectClassCode
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
            int studentId = (int)classAdmissionDetails.StudentId;
            int classId = (int)classAdmissionDetails.ClassId;

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

        public async Task<ApiResponseDTO<List<User>>> GetClassEnrollmentRequests(int classId)
        {
            Class c = await _eClassroomRepository.GetClassById(classId);

            if (c == null)
            {
                return new ApiResponseDTO<List<User>> { Success = false, ResponseType = ResponseType.ClassNotFound };
            }

            List<User> enrollmentRequests = await _eClassroomRepository.GetClassEnrollmentRequests(classId);
            return new ApiResponseDTO<List<User>>
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


        public async Task<ApiResponseDTO<GetExerciseDTO>> GetExerciseById(int exerciseId)
        {
            Exercise? exercise = await _eClassroomRepository.GetExerciseById(exerciseId);

            if (exercise == null)
            {
                return new ApiResponseDTO<GetExerciseDTO>
                {
                    Success = false,
                    ResponseType = ResponseType.ExerciseNotFound
                };
            }

            List<ExerciseContent> exerciseContents = await _eClassroomRepository.GetExerciseContents(exercise.BatchId);
            bool isMultipleChoice = false;

            GetExerciseDTO projectedExercise = new()
            {
                ExerciseTitle = exercise.ExerciseTitle,
                ExerciseDescription = exercise.ExerciseDescription,
                CreatedAt = exercise.CreatedAt,
                ExerciseItems = exercise.ExerciseItems.Select(item =>
                {
                    ExerciseContent content = exerciseContents.FirstOrDefault(c => c.ItemNumber == item.ItemNumber && c.BatchId == exercise.BatchId);

                    if (item is MultipleChoiceItem multipleChoiceItem)
                    {
                        isMultipleChoice = true;
                        return new GetExerciseItemDTO
                        {
                            ItemNumber = item.ItemNumber,
                            Question = item.Question,
                            CorrectAnswer = item.CorrectAnswer,
                            PresignedURL = _s3Service.GeneratePresignedFetchExerciseContentUrl(content?.ContentS3Key),
                            ChoiceA = multipleChoiceItem.ChoiceA,
                            ChoiceB = multipleChoiceItem.ChoiceB,
                            ChoiceC = multipleChoiceItem.ChoiceC,
                            ChoiceD = multipleChoiceItem.ChoiceD
                        };
                    }
                    else
                    {
                        return new GetExerciseItemDTO
                        {
                            ItemNumber = item.ItemNumber,
                            Question = item.Question,
                            CorrectAnswer = item.CorrectAnswer,
                            PresignedURL = _s3Service.GeneratePresignedFetchExerciseContentUrl(content?.ContentS3Key),
                        };
                    }
                }).ToList(),
            };

            projectedExercise.ExerciseType = isMultipleChoice ? "MultipleChoice" : "Base";

            return new ApiResponseDTO<GetExerciseDTO>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = projectedExercise
            };
        }

        public async Task<ApiResponseDTO<List<Exercise>>> GetTeacherExercises(int teacherId)
        {
            List<Exercise> exercises = await _eClassroomRepository.GetTeacherExercises(teacherId);
            return new ApiResponseDTO<List<Exercise>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = exercises
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

        public async Task<ApiResponseDTO> EditExerciseItem(EditExerciseItemDTO exerciseItem)
        {
            await _eClassroomRepository.EditExerciseItem(exerciseItem);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.ExerciseItemEditSuccessful
            };
        }

        public async Task<ApiResponseDTO> CreateBatchExerciseContent(List<CreateExerciseContentDTO> exerciseContents)
        {
            await _eClassroomRepository.CreateBatchExerciseContent(exerciseContents);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.ExerciseContentCreated
            };
        }

        public async Task<ApiResponseDTO<ContentS3KeyDTO>> GetContentS3Key(GetContentS3KeyDTO info)
        {
            ContentS3KeyDTO contentKey = await _eClassroomRepository.GetContentS3Key(info.BatchId, info.ItemNumber);

            return new ApiResponseDTO<ContentS3KeyDTO>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = contentKey
            };
        }

        public async Task<ApiResponseDTO> CreateBatchExerciseItemAnswer(int classExerciseId, int studentId, List<ExerciseItemAnswerDTO> exerciseItemAnswers)
        {
            await _eClassroomRepository.CreateBatchExerciseItemAnswer(classExerciseId, studentId, exerciseItemAnswers);
            return new ApiResponseDTO
            {
                Success = true,
                ResponseType = ResponseType.ResourceGroupCreated
            };
        }

        public async Task<ApiResponseDTO<List<ExerciseItemAnswerDTO>>> GetBatchExerciseItemAnswer(int classExerciseId, int studentId)
        {
            List<ExerciseItemAnswerDTO> exerciseItemAnswers = await _eClassroomRepository.GetBatchExerciseItemAnswer(classExerciseId, studentId);
            return new ApiResponseDTO<List<ExerciseItemAnswerDTO>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = exerciseItemAnswers
            };
        }

        public async Task<ApiResponseDTO> AssignExerciseToClass(int classId, int exerciseId)
        {
            ClassExercise existingClassExercise = await _eClassroomRepository.CheckIfClassExerciseAlreadyExists(classId, exerciseId);

            if (existingClassExercise != null)
            {
                return new ApiResponseDTO
                {
                    Success = false,
                    ResponseType = ResponseType.ClassExerciseAlreadyExists
                };
            }

            await _eClassroomRepository.CreateClassExercise(classId, exerciseId);
            return new()
            {
                Success = true,
                ResponseType = ResponseType.ClassExerciseSuccessfullyCreated
            };
        }

        public async Task<ApiResponseDTO<List<ClassExerciseDTO>>> GetClassExercises(int classId)
        {
            List<ClassExerciseDTO> classExercises = await _eClassroomRepository.GetClassExercises(classId);
            return new()
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = classExercises
            };
        }

        public async Task<ApiResponseDTO<List<GetUnassignedExerciseDTO>>> GetUnassignedClassExercises(int classId, int teacherId)
        {
            List<GetUnassignedExerciseDTO> unassignedExercises = await _eClassroomRepository.GetUnassignedClassExercises(classId, teacherId);
            return new()
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = unassignedExercises
            };
        }
    }
}
