using Microsoft.AspNetCore.Mvc;
using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/statistics")] // Базовый маршрут: /api/statistics
    public class StatisticsController : ControllerBase
    {
        private readonly IZooStatisticsService _statisticsService;

        public StatisticsController(IZooStatisticsService statisticsService)
        {
            _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        }

        
        [HttpGet] // GET api/statistics
        [ProducesResponseType(typeof(ZooStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<ZooStatisticsDto>> GetStatistics()
        {
            try
            {
                var statistics = await _statisticsService.GetStatisticsAsync();
                return Ok(statistics); 
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving statistics.");
            }
        }
    }
}