using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IGoogleAuthService _googleAuthService;


        public AuthController(IAuthService authService, IJwtService jwtService, IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
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
        [AllowAnonymous]
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
                Roles = response?.Data.Roles
            });

            return Ok(new
            {
                Token = token,
                Response = response
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Only users with "Admin" role can access this
        [HttpGet]
        [Route("admin-only/")]
        public IActionResult AdminOnlyEndpoint()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Access denied: You must log in first." });
            }

            return Ok("You are authenticated!");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("login-google/")]
        public async Task<IActionResult> ValidateUserWithGoogle()
        {
            var properties = _googleAuthService.GetAuthProperties();
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                var userInfo = await _googleAuthService.GetUserInfoAsync(HttpContext);

                if (userInfo == null || !userInfo.ContainsKey("Email"))
                {
                    return BadRequest(new
                    {
                        Message = "Google authentication failed: No user information retrieved.",
                        Details = "Ensure the user has granted access and Google API is returning valid user data."
                    });
                }

                var token = _jwtService.GenerateToken(new AuthTokenRequestDTO
                {
                    Email = userInfo["Email"],
                    Roles = new List<string> { "User" }
                });

                return Ok(new
                {
                    Message = "Google authentication successful",
                    User = userInfo,
                    Token = token
                });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new
                {
                    Message = "Authentication process failed.",
                    Details = "Possible causes: Invalid token, expired session, or incorrect authentication flow.",
                    Error = ex.Message
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    Message = "External authentication service unreachable.",
                    Details = "Google's authentication service might be down, or there may be a network issue.",
                    Error = ex.Message
                });
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Details = "Check server logs for more information.",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }

    }
}
