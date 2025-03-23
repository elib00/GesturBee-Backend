using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register/")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDTO user)
        {
            ApiResponseDTO<UserDetailsDTO> response = await _authService.RegisterUser(user);
            if (!response.Success)
            {
                switch (response.ResponseType)
                {
                    case ResponseType.MissingInput:
                        return BadRequest(response);
                    case ResponseType.UserAlreadyExists:
                        return Conflict(response);
                }
            }

            return Ok(response);
        }

        //[HttpPost("login/")] //shorter ni sha
        [HttpPost]
        [Route("login/")]
        public async Task<IActionResult> ValidateUser([FromBody] UserValidationDTO credentials)
        {
            ApiResponseDTO<UserDetailsDTO> response = await _authService.ValidateUser(credentials);
            if (!response.Success)
            {
                switch (response.ResponseType)
                {
                    case ResponseType.MissingInput:
                        return BadRequest(response);
                    case ResponseType.UserNotFound:
                        return NotFound(response);
                    case ResponseType.InvalidUser:
                        return Unauthorized(response);
                }
            }

            string token = _jwtService.GenerateToken(new AuthTokenRequestDTO
            {
                Email = response?.Data.Email,
                Role = "User"
            });

            return Ok(new
            {
                Token = token,
                Response = response
            });
        }

        [Authorize] // Only users with "Admin" role can access this
        [HttpGet]
        [Route("admin-only/")]
        public IActionResult AdminOnlyEndpoint()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Access denied: You mus t log in first." });
            }

            return Ok("You are authenticated!");
        }

        [HttpPost]
        [Route("login-google/")]
        public async Task<IActionResult> ValidateUserWithGoogle([FromBody] string token)
        {
            return Ok();
        }

    }
}
