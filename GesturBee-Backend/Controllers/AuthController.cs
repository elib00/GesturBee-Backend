using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GesturBee_Backend.Models;
using Azure;
using GesturBee_Backend.Services;
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


        public AuthController(IAuthService authService, IJwtService jwtService, IExternalAuthServiceFactory externalAuthServiceFactory)
        {
            _authService = authService;
            _jwtService = jwtService;
            _externalAuthServiceFactory = externalAuthServiceFactory;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register/")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDTO user)
        {
            ApiResponseDTO<UserDetailsDTO> response = await _authService.RegisterUser(user, AuthType.LocalAuth);
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
            var externalAuthService = _externalAuthServiceFactory.GetAuthService("Google");
            var properties = externalAuthService.GetAuthProperties();
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService("Google");
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

                //check if the user has a local account
                ApiResponseDTO<UserDetailsDTO> fetchUserByEmailResponse = await _authService.FetchUserUsingEmail(userInfo["Email"]);

                //meaning the user doesn't have a local account, create an account
                if(!fetchUserByEmailResponse.Success && fetchUserByEmailResponse.ResponseType == ResponseType.UserNotFound)
                {
                    //destructure the name of the user
                    string fullName = userInfo["Name"];
                    string[] nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    ApiResponseDTO<UserDetailsDTO> responseForRegister = await _authService.RegisterUser(new UserRegistrationDTO
                    {
                        Email = userInfo["Email"],
                        FirstName = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : nameParts[0],
                        LastName = nameParts.Length > 1 ? nameParts[^1] : "",
                    }, AuthType.GoogleAuth);

                    if (!responseForRegister.Success)
                    {
                        switch (responseForRegister.ResponseType)
                        {
                            case ResponseType.MissingInput:
                                return BadRequest(responseForRegister);
                            case ResponseType.UserAlreadyExists:
                                return Conflict(responseForRegister);
                        }
                    }

                    return Ok(new
                    {
                        Message = "Google authentication successful",
                        Response = responseForRegister,
                        Token = token
                    });
                }

                return Ok(new
                {
                    Message = "Google authentication successful",
                    Response = fetchUserByEmailResponse,
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
        [HttpGet]
        [Route("login-facebook/")]
        public async Task<IActionResult> ValidateUserWithFacebook()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService("Facebook");
            var properties = externalAuthService.GetAuthProperties();
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("facebook-callback")]
        public async Task<IActionResult> FacebookCallback()
        {
            var externalAuthService = _externalAuthServiceFactory.GetAuthService("Facebook");
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

                //check if the user has a local account
                ApiResponseDTO<UserDetailsDTO> fetchUserByEmailResponse = await _authService.FetchUserUsingEmail(userInfo["Email"]);

                //meaning the user doesn't have a local account, create an account
                if (!fetchUserByEmailResponse.Success && fetchUserByEmailResponse.ResponseType == ResponseType.UserNotFound)
                {
                    //destructure the name of the user
                    string fullName = userInfo["Name"];
                    string[] nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    ApiResponseDTO<UserDetailsDTO> responseForRegister = await _authService.RegisterUser(new UserRegistrationDTO
                    {
                        Email = userInfo["Email"],
                        FirstName = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : nameParts[0],
                        LastName = nameParts.Length > 1 ? nameParts[^1] : "",
                    }, AuthType.GoogleAuth);

                    if (!responseForRegister.Success)
                    {
                        switch (responseForRegister.ResponseType)
                        {
                            case ResponseType.MissingInput:
                                return BadRequest(responseForRegister);
                            case ResponseType.UserAlreadyExists:
                                return Conflict(responseForRegister);
                        }
                    }

                    return Ok(new
                    {
                        Message = "Facebook authentication successful",
                        Response = responseForRegister,
                        Token = token
                    });
                }

                return Ok(new
                {
                    Message = "Facebook authentication successful",
                    Response = fetchUserByEmailResponse,
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
    }
}
