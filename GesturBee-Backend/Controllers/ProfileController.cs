using GesturBee_Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/profile/")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> EditProfile([FromBody] UserProfileDTO profile)
        {
            ApiResponseDTO<UserProfileDTO> response = await _profileService.EditUserProfile(profile);

            if (!response.Success)
            {
                switch (response.ResponseType)
                {
                    case Enums.ResponseType.ProfileNotFound:
                        return NotFound(response);
                }
            }

            return Ok(response);
        }
    }
}
