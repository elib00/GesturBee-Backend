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
        [HttpPost("class/add-student/")]

        public async Task<IActionResult> AddStudentToClass([FromBody] StudentAndClassDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int studentId = (int) request.StudentId;
            int classId = (int) request.ClassId;

            ApiResponseDTO<object> response = await _eClassroomService.AddStudentToClass(request);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/invite-student/")]
        public async Task<IActionResult> InviteStudentToClass([FromBody] StudentAndClassDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponseDTO<object> response = await _eClassroomService.InviteStudentToClass(request);

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/create-class/")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassDTO info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
        [HttpPost("class/request-enrollment/")]
        public async Task<IActionResult> RequestClassEnrollment([FromBody] StudentAndClassDTO info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponseDTO<object> response = await _eClassroomService.RequestClassEnrollment(info);

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponseDTO<object> response = await _eClassroomService.ProcessEnrollmentRequest(classAdmissionDetails);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/process-invitation/")]
        public async Task<IActionResult> ProcessInvitationRequest([FromBody] ClassAdmissionDTO classAdmissionDetails)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponseDTO<object> response = await _eClassroomService.ProcessInvitationRequest(classAdmissionDetails);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("teacher/{teacherId}/class-enrollments")]
        public async Task<IActionResult> GetTeacherClassEnrollmentRequests([FromRoute] int teacherId)
        {
            ApiResponseDTO<List<ClassEnrollmentGroupDTO>> response = await _eClassroomService.GetTeacherClassEnrollmentRequests(teacherId);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("student/{studentId}/class-invitations")]
        public async Task<IActionResult> GetStudentClassInvitationRequests([FromRoute] int studentId)
        {
            ApiResponseDTO<List<ClassInvitationGroupDTO>> response = await _eClassroomService.GetStudentClassInvitationRequests(studentId);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("class/remove-student")]
        public async Task<IActionResult> RemoveStudentFromClass([FromBody] StudentAndClassDTO info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponseDTO<object> response = await _eClassroomService.RemoveStudentFromClass(info);

            if (!response.Success)
            {
                return NotFound(info.StudentId);
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
