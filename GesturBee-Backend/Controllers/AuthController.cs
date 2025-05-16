using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Google.Apis.Auth;
using GesturBee_Backend.Validators;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly GoogleTokenValidator _googleTokenValidator;
        private readonly FacebookTokenValidator _facebookTokenValidator;
        private readonly IEmailService _emailService;


        public AuthController(IAuthService authService, IJwtService jwtService, GoogleTokenValidator googleTokenValidator, 
            FacebookTokenValidator facebookTokenValidator, IEmailService emailService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _googleTokenValidator = googleTokenValidator;
            _facebookTokenValidator = facebookTokenValidator;
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
        [HttpPost("external-login/google/")]
        public async Task<IActionResult> ExternalLoginWithGoogle([FromBody] GoogleAuthDTO authCredentials)
        {
            GoogleJsonWebSignature.Payload payload = await _googleTokenValidator.ValidateAsync(authCredentials.IdToken);
           

            if(payload == null || string.IsNullOrEmpty(payload.Email))
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.InvalidGoogleToken
                }); 
            }

            //check if the user has a local account
            ApiResponseDTO<UserDetailsDTO> response = await _authService.ProcessGoogleAuth(new Dictionary<string, string>
            {
                { "Email", payload.Email },
                { "FirstName", payload.GivenName },
                { "LastName", payload.FamilyName }
            });


            string token = _jwtService.GenerateToken(new AuthTokenRequestDTO
            {
                Email = response?.Data.Email,
                Roles = response?.Data.Roles,
                Type = "google-auth"
            });

            return Ok(new
            {
                Token = token,
                Response = response
            });

        }

        [AllowAnonymous]
        [HttpPost("external-login/facebook/")]
        public async Task<IActionResult> ExternalLoginWithFacebook([FromBody] FacebookAuthDTO authCredentials)
        {
            FacebookUserInfoDTO userInfo = await _facebookTokenValidator.ValidateTokenAsync(authCredentials.AccessToken);
            if(userInfo == null)
            {
                return Unauthorized(new ApiResponseDTO<object>
                {
                    Success = false,
                    ResponseType = ResponseType.InvalidFacebookToken,
                });
            }

            ApiResponseDTO<UserDetailsDTO> response = await _authService.ProcessFacebookAuth(userInfo);

            string token = _jwtService.GenerateToken(new AuthTokenRequestDTO
            {
                Email = response?.Data.Email,
                Roles = response?.Data.Roles,
                Type = "facebook-auth"
            });

            return Ok(new
            {
                Token = token,
                Response = response
            });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("verify-reset-password/")]
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
