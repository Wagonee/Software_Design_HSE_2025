using Zoo.Domain.Entities;

namespace Zoo.Domain.Interfaces;

public interface IAnimalRepository
{
    Task<Animal>? GetAnimalByIdAsync(Guid animalId);
    Task<IEnumerable<Animal>> GetAllAnimalsAsync();
    Task AddAsync(Animal animal);
    Task UpdateAsync(Animal animal);
    Task DeleteAsync(Guid animal);
}