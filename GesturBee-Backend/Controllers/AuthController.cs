using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IExternalAuthServiceFactory _externalAuthServiceFactory;
        private readonly IEmailService _emailService;


        public AuthController(IAuthService authService, IJwtService jwtService, IExternalAuthServiceFactory externalAuthServiceFactory, IEmailService emailService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _externalAuthServiceFactory = externalAuthServiceFactory;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("register/")]
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
        [HttpPost("login/")]
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

        [AllowAnonymous]
        [HttpGet("login-google/")]
        public async Task<IActionResult> ValidateUserWithGoogle()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService(AuthType.GoogleAuth);
            var properties = externalAuthService.GetAuthProperties();
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService(AuthType.GoogleAuth);
            try
            {
                var userInfo = await externalAuthService.GetUserInfoAsync(HttpContext);

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

                //destructure the name of the user

                ApiResponseDTO<UserDetailsDTO> response = await _authService.ProcessExternalAuth(userInfo);

                return Ok(new
                {
                    Response = response, 
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

        [AllowAnonymous]
        [HttpGet("login-facebook/")]
        public async Task<IActionResult> ValidateUserWithFacebook()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService(AuthType.FacebookAuth);
            var properties = externalAuthService.GetAuthProperties();
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService(AuthType.FacebookAuth);
            try
            {
                var userInfo = await externalAuthService.GetUserInfoAsync(HttpContext);

                if (userInfo == null || !userInfo.ContainsKey("Email"))
                {
                    return BadRequest(new
                    {
                        Message = "Facebook authentication failed: No user information retrieved.",
                        Details = "Ensure the user has granted access and Facebook API is returning valid user data."
                    });
                }

                var token = _jwtService.GenerateToken(new AuthTokenRequestDTO
                {
                    Email = userInfo["Email"],
                    Roles = new List<string> { "User" }
                });

                ApiResponseDTO<UserDetailsDTO> response = await _authService.ProcessExternalAuth(userInfo);

                return Ok(new
                {
                    Response = response,
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
                    Details = "Facebook's authentication service might be down, or there may be a network issue.",
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


        [HttpGet("google-logout")]
        public async Task<IActionResult> GoogleLogout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpGet("facebook-logout")]
        public async Task<IActionResult> FacebookLogout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("verify-reset-password")]
        public async Task<IActionResult> VerifyUserBeforeResetPassword([FromBody] VerifyChangePasswordDTO details)
        {
            string userEmail = details.Email;
            string userPassword = details.Password;

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword))
            {
                return BadRequest(new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.MissingInput
                });
            }

            //check if user exists (just double checking)
            ApiResponseDTO<UserDetailsDTO> response = await _authService.ValidateUser(new UserValidationDTO
            {
                Email = userEmail,
                Password = userPassword
            });

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
                Roles = response?.Data.Roles,
                Type = "password-reset"
            });

            //return Ok(new
            //{
            //    Token = token,
            //    Response = response
            //});

            string toEmail = userEmail;
            string subject = "testing";
            string body = $"https://localhost:7152/api/auth/verify-reset-password-token?token={token}";

            bool emailSent = await _emailService.SendEmailAsync(toEmail, subject, body);

            return Ok(new
            {
                Token = token
            });
        }


        [HttpGet("verify-reset-password-token")]
        public async Task<IActionResult> VerifyResetPasswordToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }

            // Validate token, show password reset form, etc.
            // return Ok(new { message = "Token received", token });

            ResponseType isPasswordResetTokenValid = _jwtService.ValidatePasswordResetToken(token);

            switch(isPasswordResetTokenValid)
            {
                case ResponseType.InvalidToken:
                    return BadRequest(new ApiResponseDTO<object>
                    {
                        Success = false,
                        ResponseType = ResponseType.InvalidToken
                    });
                case ResponseType.TokenMissingRequiredClaim:
                    return BadRequest(new ApiResponseDTO<object>
                    {
                        Success = false,
                        ResponseType = ResponseType.TokenMissingRequiredClaim
                    });
            }

            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.ValidToken
            });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO details)
        {
            //reset the pw
            ApiResponseDTO<object> response = await _authService.ResetPassword(details);

            if (!response.Success)
            {
                if(response.ResponseType == ResponseType.MissingInput || response.ResponseType == ResponseType.ResetPasswordMismatch)
                {
                    return BadRequest(response);
                }
            }

            return Ok(new ApiResponseDTO<object>
            {
                Success = true,
                ResponseType = ResponseType.PasswordResetSuccessful
            });
        }
    }
}
