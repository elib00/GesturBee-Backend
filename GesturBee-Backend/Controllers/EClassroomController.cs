using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GesturBee_Backend.Controllers
{

    [ApiController]
    [Route("api/e-classroom/")]
    public class EClassroomController : ControllerBase
    {
        private readonly IEClassroomService _eClassroomService;
        
        public EClassroomController(IEClassroomService eClassroomService)
        {
            _eClassroomService = eClassroomService;
        }

        public async Task<IActionResult> GetStudentClasses([FromQuery] int studentId)
        {
            ApiResponseDTO<List<Class>> response = await _eClassroomService.GetStudentClasses(studentId);
            return Ok(response);
        }

        public async Task<IActionResult> GetClassById([FromQuery] int classId)
        {
            ApiResponseDTO<Class> response = await _eClassroomService.GetClassById(classId);
            return Ok(response);
        }

        public async Task<IActionResult> GetTeacherClasses([FromQuery] int teacherId)
        {
            ApiResponseDTO<List<Class>> response = await _eClassroomService.GetTeacherClasses(teacherId);
            return Ok(response);
        }

        public async Task<IActionResult> GetClassStudents([FromQuery] int classId)
        {
            ApiResponseDTO<List<Student>> response = await _eClassroomService.GetClassStudents(classId);
            return Ok(response);
        }

        public async Task<IActionResult> AddStudentToClass([FromBody] StudentAndClassDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int studentId = (int) request.StudentId;
            int classId = (int) request.ClassId;

            ApiResponseDTO<object> response = await _eClassroomService.AddStudentToClass(studentId, classId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status201Created, response);
        }

        public async Task<IActionResult> InviteStudentToClass([FromBody] StudentAndClassDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int studentId = (int)request.StudentId;
            int classId = (int)request.ClassId;

            ApiResponseDTO<object> response = await _eClassroomService.InviteStudentToClass(studentId, classId);\

            if (!response.Success)
            {
                if(response.ResponseType == ResponseType.StudentAlreadyInvited)
                {
                    return Conflict(response);
                }
                else
                {
                    return NotFound(response);
                }
            }

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
