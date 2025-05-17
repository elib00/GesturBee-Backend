using GesturBee_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GesturBee_Backend.Controllers
{
    [ApiController]
    [Route("api/roadmap")]
    public class RoadmapController : ControllerBase
    {
        private readonly IRoadmapService _roadmapService;

        public RoadmapController(IRoadmapService roadmapService)
        {
            _roadmapService = roadmapService;
        }
    }
}
