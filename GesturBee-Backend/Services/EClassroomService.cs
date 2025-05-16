using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

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
        
        public async Task<ApiResponseDTO<List<Student>>> GetClassStudents(int classId)
        {
            List<Student> students = await _eClassroomRepository.GetClassStudents(classId);
            return new ApiResponseDTO<List<Student>>
            {
                Success = true,
                ResponseType = ResponseType.SuccessfulRetrievalOfResource,
                Data = students
            };
        }

        public async Task<ApiResponseDTO<object>> AddStudentToClass(StudentAndClassDTO info)
        {
            int studentId = (int)info.StudentId;
            int classId = (int)info.StudentId;

            Student student = await _eClassroomRepository.GetStudentById(studentId);

            if (student == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.StudentNotFound };
            }


            Class cls = await _eClassroomRepository.GetClassById(classId);

            if (cls == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.ClassNotFound };
            }

            await _eClassroomRepository.AddStudentToClass(student, cls);

            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.StudentAddedToClassroom
            };
        }

        public async Task<ApiResponseDTO<object>> InviteStudentToClass(StudentAndClassDTO info)
        {
            int studentId = (int)info.StudentId;
            int classId = (int)info.StudentId;

            Student student = await _eClassroomRepository.GetStudentById(studentId);

            if (student == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.StudentNotFound };
            }


            Class cls = await _eClassroomRepository.GetClassById(classId);

            if (cls == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.ClassNotFound };
            }

            //check if invitation is already present

            bool isAlreadyInvited = await _eClassroomRepository.StudentAlreadyInvited(studentId, classId);

            if(isAlreadyInvited)
            {
                return new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.StudentAlreadyInvited
                };
            }

            await _eClassroomRepository.InviteStudentToClass(student, cls);

            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.StudentInviteSuccessful
            };
        }

        public async Task<ApiResponseDTO<object>> CreateClass(CreateClassDTO info)
        {
            int teacherId = info.TeacherId;
            Teacher teacher = await _eClassroomRepository.GetTeacherById(teacherId);

            if (teacher == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.TeacherNotFound };
            }

            bool isNameAlreadyTaken = await _eClassroomRepository.ClassNameAlreadyTaken(info.ClassName);

            if(isNameAlreadyTaken)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.ClassNameAlreadyTaken };
            }

  
            await _eClassroomRepository.CreateClass(info);

            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.ClassCreated
            };
        }

        public async Task<ApiResponseDTO<object>> RequestClassEnrollment(StudentAndClassDTO info)
        {
            int studentId = (int) info.StudentId;
            int classId =  (int) info.StudentId;

            Student student = await _eClassroomRepository.GetStudentById(studentId);

            if (student == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.StudentNotFound };
            }


            Class cls = await _eClassroomRepository.GetClassById(classId);

            if (cls == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.ClassNotFound };
            }

            bool hasAlreadyRequested = await _eClassroomRepository.RequestForClassEnrollmentAlreadySent(studentId, classId);

            if(hasAlreadyRequested)
            {
                return new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.EnrollmentRequestAlreadySent
                };
            }

            await _eClassroomRepository.RequestClassEnrollment(student, cls);
            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.EnrollmentRequestSuccessful
            };
        }

    }
}
