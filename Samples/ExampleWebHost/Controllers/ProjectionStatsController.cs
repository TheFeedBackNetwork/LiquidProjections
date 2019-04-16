using LiquidProjections.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWebHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionStatsController : ControllerBase
    {
        private readonly ProjectionStats _projectionStats;
        public ProjectionStatsController(ProjectionStats projectionStats)
        {
            _projectionStats = projectionStats;
        }

        [HttpGet]
        public ActionResult<ProjectionStats> Get()
        {
            return _projectionStats;
        }
    }
}
