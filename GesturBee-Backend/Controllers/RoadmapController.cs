﻿using GesturBee_Backend.DTO;
using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/roadmap/")]
    public class RoadmapController : ControllerBase
    {
        private readonly IRoadmapService _roadmapService;

        public RoadmapController(IRoadmapService roadmapService)
        {
            _roadmapService = roadmapService;
        }

        [HttpGet("health/")]
        public async Task<IActionResult> CheckHealth()
        {
            return Ok(new
            {
                Message = "Connected"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch("user/{userId}/edit-progress/")]
        public async Task<IActionResult> EditRoadmapProgress([FromRoute] int userId, [FromBody] RoadmapProgressDTO newProgress)
        {
            ApiResponseDTO response = await _roadmapService.EditRoadmapProgress(userId, newProgress);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return StatusCode(StatusCodes.Status204NoContent, response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("user/{userId}/progress/")]
        public async Task<IActionResult> GetUserCurrentProgress([FromRoute] int userId)
        {
            ApiResponseDTO<RoadmapProgressDTO> response = await _roadmapService.GetRoadmapProgressWithUserId(userId);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


    }
}
