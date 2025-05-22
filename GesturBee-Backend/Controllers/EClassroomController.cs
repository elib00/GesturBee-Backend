using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GesturBee_Backend.Controllers
{

    [ApiController]
    [Route("api/e-classroom/")]
    public class EClassroomController : ControllerBase
    {
        private readonly IEClassroomService _eClassroomService;
        private readonly IS3Service _s3Service;

        public EClassroomController(IEClassroomService eClassroomService, IS3Service s3Service)
        {
            _eClassroomService = eClassroomService;
            _s3Service = s3Service;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("student/{studentId}/classes/")]
        public async Task<IActionResult> GetStudentClasses([FromRoute] int studentId)
        {
            ApiResponseDTO<List<Class>> response = await _eClassroomService.GetStudentClasses(studentId);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("class/{classId}/")]
        public async Task<IActionResult> GetClassById([FromRoute] int classId)
        {
            ApiResponseDTO<Class> response = await _eClassroomService.GetClassById(classId);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("teacher/{teacherId}/classes/")]

        public async Task<IActionResult> GetTeacherClasses([FromRoute] int teacherId)
        {
            ApiResponseDTO<List<Class>> response = await _eClassroomService.GetTeacherClasses(teacherId);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("class/{classId}/students/")]
        public async Task<IActionResult> GetClassStudents([FromRoute] int classId)
        {
            ApiResponseDTO<List<User>> response = await _eClassroomService.GetClassStudents(classId);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/{classId}/add-student/{studentId}/")]

        public async Task<IActionResult> AddStudentToClass([FromRoute] int studentId, [FromRoute] int classId)
        {
            ApiResponseDTO response = await _eClassroomService.AddStudentToClass(studentId, classId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/create-class/")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassDTO info)
        {
            ApiResponseDTO<object> response = await _eClassroomService.CreateClass(info);

            if(!response.Success)
            {
                if(response.ResponseType == ResponseType.ClassNameAlreadyTaken)
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/{classId}/request-enrollment/{studentId}/")]
        public async Task<IActionResult> RequestClassEnrollment([FromRoute] int studentId, [FromRoute] int classId)
        {
            ApiResponseDTO<object> response = await _eClassroomService.RequestClassEnrollment(studentId, classId);

            if (!response.Success)
            {
                if (response.ResponseType == ResponseType.EnrollmentRequestAlreadySent)
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/process-enrollment/")]
        public async Task<IActionResult> ProcessEnrollmentRequest([FromBody] ClassAdmissionDTO classAdmissionDetails)
        {
            ApiResponseDTO<object> response = await _eClassroomService.ProcessEnrollmentRequest(classAdmissionDetails);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/{classId}/enrollments-requests")]
        public async Task<IActionResult> GetClassEnrollmentRequests([FromRoute] int classId)
        {
            ApiResponseDTO<ICollection<User>> response = await _eClassroomService.GetClassEnrollmentRequests(classId);
            return Ok(response);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/{classId}/remove-student/{studentId}/")]
        public async Task<IActionResult> RemoveStudentFromClass([FromRoute] int studentId, [FromRoute] int classId)
        {
            ApiResponseDTO<object> response = await _eClassroomService.RemoveStudentFromClass(studentId, classId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("upload-presigned-url/")]
        public async Task<IActionResult> GetUploadPresignedURL([FromBody] UploadRequestDTO uploadRequest)
        {
            if (string.IsNullOrEmpty(uploadRequest.FileName) || string.IsNullOrEmpty(uploadRequest.ContentType))
                return BadRequest("FileName and ContentType are required.");

            string url = _s3Service.GeneratePreSignedUploadUrl(uploadRequest.FileName, uploadRequest.ContentType);

            return Ok(new { Url = url });
        }
    }
}
