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

        public async Task<ApiResponseDTO<object>> AddStudentToClass(int studentId, int classId)
        {
            Student student = await _eClassroomRepository.GetStudentById(studentId);

            if (student == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.StudentNotFound };
            }


            Class cls = await _eClassroomRepository.GetClassById(classId);

            if (cls == null)
            {
                return new ApiResponseDTO<object> { Success = false, ResponseType = ResponseType.StudentNotFound };
            }

            await _eClassroomRepository.AddStudentToClass(student, cls);

            return new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.StudentAddedToClassroom
            };
        }

    }
}
