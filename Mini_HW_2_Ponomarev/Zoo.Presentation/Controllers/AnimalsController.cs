using Microsoft.AspNetCore.Mvc;
using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;

namespace Zoo.Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;

    public AnimalsController(IAnimalService animalService)
    {
        _animalService = animalService ?? throw new ArgumentNullException(nameof(animalService));
    }

    [HttpGet] // GET api/animals
    [ProducesResponseType(typeof(IEnumerable<AnimalDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AnimalDto>>> GetAllAnimals()
    {
        var animals = await _animalService.GetAllAnimalsAsync();
        return Ok(animals);
    }
    
    [HttpGet("{id:guid}")] // GET api/animal/{id} 
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnimalDto>> GetAnimalById(Guid id)
    {
        var animal = await _animalService.GetAnimalByIdAsync(id);
        if (animal == null)
        {
            return NotFound($"Animal with ID {id} not found.");
        }
        return Ok(animal); 
    }
    
    
    [HttpPost] // POST api/animals
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status201Created)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    public async Task<ActionResult<AnimalDto>> AddAnimal([FromBody] CreateAnimalDto createDto)
    {
        try
        {
            var createdAnimal = await _animalService.AddAnimalAsync(createDto);
            return CreatedAtAction(nameof(GetAnimalById), new { id = createdAnimal.Id }, createdAnimal);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex) 
        {
            return BadRequest(ex.Message);
        }
    }

        
    [HttpDelete("{id:guid}")] // DELETE api/animals/{id}
    [ProducesResponseType(StatusCodes.Status204NoContent)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnimal(Guid id)
    {
        var success = await _animalService.DeleteAnimalAsync(id);
        if (!success)
        {
            return NotFound($"Animal with ID {id} not found.");
        }
        return NoContent();
    }
        
        
    [HttpPost("{animalId:guid}/move/{targetEnclosureId:guid}")] // POST api/animals/{animalId}/move/{targetEnclosureId}
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    public async Task<IActionResult> MoveAnimal(Guid animalId, Guid targetEnclosureId)
    {
        try
        {
            await _animalService.MoveAnimalAsync(animalId, targetEnclosureId);
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
    }

     
    [HttpPost("{animalId:guid}/feed")] // POST api/animals/{animalId}/feed
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FeedAnimal(Guid animalId)
    {
        try
        {
            await _animalService.FeedAnimalAsync(animalId, DateTime.UtcNow);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

         
    [HttpPost("{animalId:guid}/heal")] // POST api/animals/{animalId}/heal
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HealAnimal(Guid animalId)
    {
        try
        {
            await _animalService.HealAnimalAsync(animalId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}