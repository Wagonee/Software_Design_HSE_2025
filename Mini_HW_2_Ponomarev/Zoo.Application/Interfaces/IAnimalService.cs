using Zoo.Application.DTOs;

namespace Zoo.Application.Interfaces
{
    public interface IAnimalService
    {
        Task<AnimalDto?> GetAnimalByIdAsync(Guid id);
        
        Task<IEnumerable<AnimalDto>> GetAllAnimalsAsync();
        
        Task<AnimalDto> AddAnimalAsync(CreateAnimalDto createAnimalDto);
        
        Task<bool> DeleteAnimalAsync(Guid id);

        Task MoveAnimalAsync(Guid animalId, Guid targetEnclosureId);
        
        Task FeedAnimalAsync(Guid animalId, DateTime feedingTime);

        Task HealAnimalAsync(Guid animalId);
    }
}