using Microsoft.AspNetCore.Mvc;
using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/feeding")] 
    public class FeedingController : ControllerBase
    {
        private readonly IFeedingService _feedingService;

        public FeedingController(IFeedingService feedingService)
        {
            _feedingService = feedingService ?? throw new ArgumentNullException(nameof(feedingService));
        }

        
        [HttpGet] // GET api/feeding
        [ProducesResponseType(typeof(IEnumerable<FeedingScheduleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FeedingScheduleDto>>> GetAllSchedules([FromQuery] Guid? animalId)
        {
            IEnumerable<FeedingScheduleDto> schedules;
            if (animalId.HasValue)
            {
                schedules = await _feedingService.GetSchedulesByAnimalIdAsync(animalId.Value);
            }
            else
            {
                schedules = await _feedingService.GetAllSchedulesAsync();
            }
            return Ok(schedules);
        }

        
        [HttpGet("{id:guid}")] // GET api/feeding/{id}
        [ProducesResponseType(typeof(FeedingScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeedingScheduleDto>> GetScheduleById(Guid id)
        {
            var schedule = await _feedingService.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                return NotFound($"Feeding schedule with ID {id} not found.");
            }
            return Ok(schedule);
        }

        
        [HttpPost] // POST api/feeding
        [ProducesResponseType(typeof(FeedingScheduleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        public async Task<ActionResult<FeedingScheduleDto>> AddSchedule([FromBody] CreateFeedingScheduleDto createDto)
        {
            try
            {
                var createdSchedule = await _feedingService.AddScheduleAsync(createDto);
                return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.Id }, createdSchedule);
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while adding schedule.");
            }
        }

        
        [HttpDelete("{id:guid}")] // DELETE api/feeding/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            var success = await _feedingService.DeleteScheduleAsync(id);
            if (!success)
            {
                return NotFound($"Feeding schedule with ID {id} not found.");
            }
            return NoContent();
        }

        
        [HttpPut("{id:guid}/reschedule")] // PUT api/feeding/{id}/reschedule
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<IActionResult> RescheduleFeeding(Guid id, [FromBody] RescheduleFeedingDto rescheduleDto)
        {
            try
            {
                await _feedingService.RescheduleFeedingAsync(id, rescheduleDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while rescheduling.");
            }
        }

        
        [HttpPost("{id:guid}/complete")] // POST api/feeding/{id}/complete
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<IActionResult> MarkCompleted(Guid id)
        {
            try
            {
                await _feedingService.MarkFeedingCompletedAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while marking feeding as completed.");
            }
        }

        
        [HttpPost("{id:guid}/miss")] // POST api/feeding/{id}/miss
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<IActionResult> MarkMissed(Guid id)
        {
            try
            {
                await _feedingService.MarkFeedingMissedAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while marking feeding as missed.");
            }
        }
    }
}