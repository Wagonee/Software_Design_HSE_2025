using Microsoft.AspNetCore.Mvc;
using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class EnclosuresController : ControllerBase
    {
        private readonly IEnclosureService _enclosureService;

        public EnclosuresController(IEnclosureService enclosureService)
        {
            _enclosureService = enclosureService ?? throw new ArgumentNullException(nameof(enclosureService));
        }

        
        [HttpGet] // GET api/enclosures
        [ProducesResponseType(typeof(IEnumerable<EnclosureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EnclosureDto>>> GetAllEnclosures()
        {
            var enclosures = await _enclosureService.GetAllEnclosuresAsync();
            return Ok(enclosures);
        }

        
        [HttpGet("{id:guid}")] // GET api/enclosures/{id}
        [ProducesResponseType(typeof(EnclosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnclosureDto>> GetEnclosureById(Guid id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound($"Enclosure with ID {id} not found.");
            }
            return Ok(enclosure);
        }

        
        [HttpPost] // POST api/enclosures
        [ProducesResponseType(typeof(EnclosureDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EnclosureDto>> AddEnclosure([FromBody] CreateEnclosureDto createDto)
        {
            try
            {
                var createdEnclosure = await _enclosureService.AddEnclosureAsync(createDto);
                return CreatedAtAction(nameof(GetEnclosureById), new { id = createdEnclosure.Id }, createdEnclosure);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error adding enclosure: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        
        [HttpDelete("{id:guid}")] // DELETE api/enclosures/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEnclosure(Guid id)
        {
            try
            {
                var success = await _enclosureService.DeleteEnclosureAsync(id);
                if (!success) 
                {
                    return NotFound($"Enclosure with ID {id} not found.");
                }
                return NoContent(); 
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        
        [HttpPost("{id:guid}/clean")] // POST api/enclosures/{id}/clean
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CleanEnclosure(Guid id)
        {
            try
            {
                await _enclosureService.CleanEnclosureAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}