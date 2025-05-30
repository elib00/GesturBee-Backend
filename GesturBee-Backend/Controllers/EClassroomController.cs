using GesturBee_Backend.DTO;
using GesturBee_Backend.Enums;
using GesturBee_Backend.Models;
using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GesturBee_Backend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet("student/{studentId}/classes/")]
        public async Task<IActionResult> GetStudentClasses([FromRoute] int studentId)
        {
            ApiResponseDTO<List<Class>> response = await _eClassroomService.GetStudentClasses(studentId);
            return Ok(response);
        }

        [HttpGet("class/{classId}/")]
        public async Task<IActionResult> GetClassById([FromRoute] int classId)
        {
            ApiResponseDTO<Class> response = await _eClassroomService.GetClassById(classId);
            return Ok(response);
        }

        [HttpGet("teacher/{teacherId}/classes/")]
        public async Task<IActionResult> GetTeacherClasses([FromRoute] int teacherId)
        {
            ApiResponseDTO<List<Class>> response = await _eClassroomService.GetTeacherClasses(teacherId);
            return Ok(response);
        }

        [HttpGet("class/{classId}/students/")]
        public async Task<IActionResult> GetClassStudents([FromRoute] int classId)
        {
            ApiResponseDTO<List<User>> response = await _eClassroomService.GetClassStudents(classId);
            return Ok(response);
        }

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

        [HttpPost("class/{classId}/enrollments-requests")]
        public async Task<IActionResult> GetClassEnrollmentRequests([FromRoute] int classId)
        {
            ApiResponseDTO<ICollection<User>> response = await _eClassroomService.GetClassEnrollmentRequests(classId);
            return Ok(response);
        }

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

        [HttpGet("class/{classId}/users-not-enrolled/")]
        public async Task<IActionResult> GetAllUsersNotEnrolledInClass([FromRoute] int classId)
        {
            ApiResponseDTO<List<User>> response = await _eClassroomService.GetAllUsersNotEnrolledInClass(classId);
            return Ok(response);
        }

        [HttpPost("teacher/{teacherId}/exercise-content/")]
        public async Task<IActionResult> CreateBatchExerciseContent([FromBody] List<CreateExerciseContentDTO> exerciseContents)
        {
            if (exerciseContents == null || exerciseContents.Count == 0)
                return BadRequest("No exercise content provided.");

            ApiResponseDTO response = await _eClassroomService.CreateBatchExerciseContent(exerciseContents);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("upload-presigned-url/")]
        public IActionResult GetBatchUploadPresignedURL([FromBody] List<UploadRequestDTO> uploadRequests)
        {
            Dictionary<int, string> map = [];
            foreach(UploadRequestDTO uploadRequest in uploadRequests)
            {
                if (string.IsNullOrEmpty(uploadRequest.FileName) || string.IsNullOrEmpty(uploadRequest.ContentType))
                    return BadRequest("FileName and ContentType are required.");

                string url = _s3Service.GeneratePresignedClassVideoUploadUrl(uploadRequest.FileName, uploadRequest.ContentType);

                if (string.IsNullOrEmpty(url))
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate pre-signed URL.");

                map[uploadRequest.ItemNumber] = url;
            }

            return Ok(new { UrlMap = map });
        }

        [HttpGet("get-presigned-url")]
        public async Task<IActionResult> GetExerciseContentPresignedURL([FromBody] ContentS3KeyDTO key)
        {
            if (string.IsNullOrEmpty(key.Key)) 
                return BadRequest("Key is required.");

            string url = _s3Service.GeneratePresignedFetchExerciseContentUrl(key.Key);

            if (string.IsNullOrEmpty(url))
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate pre-signed URL.");

            return Ok(new { Url = url });
        }

        [HttpGet("exercise/{exerciseId}")]
        public async Task<IActionResult> GetExerciseBy([FromRoute] int exerciseId)
        {
            ApiResponseDTO<GetExerciseDTO> response = await _eClassroomService.GetExerciseById(exerciseId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("teacher/{teacherId}/exercises/")]
        public async Task<IActionResult> GetTeacherExercises([FromRoute] int teacherId)
        {
            ApiResponseDTO<List<Exercise>> response = await _eClassroomService.GetTeacherExercises(teacherId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("exercise/create-exercise")]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseDTO exercise)
        {
            ApiResponseDTO response = await _eClassroomService.CreateExercise(exercise);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPatch("exercise/item/edit-item")]
        public async Task<IActionResult> EditExerciseItem([FromBody] EditExerciseItemDTO exercise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApiResponseDTO response = await _eClassroomService.EditExerciseItem(exercise);
            return StatusCode(StatusCodes.Status204NoContent, response);
        }
    }
}
