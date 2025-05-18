using Zoo.Domain.Entities;
using Zoo.Domain.Interfaces;
using Zoo.Infrastructure.Repositories;

namespace Zoo.Infrastructure.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly Dictionary<Guid, Animal> _animals = new();
    private readonly object _lock = new();
    public Task<Animal>? GetAnimalByIdAsync(Guid animalId)
    {
        lock (_lock)
        {
            _animals.TryGetValue(animalId, out var animal);
            return Task.FromResult(animal);
        }
    }

    public Task<IEnumerable<Animal>> GetAllAnimalsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Animal>>(_animals.Values.ToList());
        }
    }

    public Task AddAsync(Animal animal)
    {
        if (animal.Id == Guid.Empty)
        {
            if (animal.Id == Guid.Empty) throw new ArgumentException("Animal ID cannot be empty.", nameof(animal));
        }

        lock (_lock)
        {
            if (_animals.ContainsKey(animal.Id))
            {
                throw new InvalidOperationException($"Animal with ID {animal.Id} already exists.");
            }
            _animals[animal.Id] = animal;
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Animal animal)
    {
        lock (_lock)
        {
            if (!_animals.ContainsKey(animal.Id))
            {
                throw new KeyNotFoundException($"Animal with ID {animal.Id} not found.");
            }
            _animals[animal.Id] = animal;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        lock (_lock)
        {
            if (!_animals.Remove(id))
            {
                throw new KeyNotFoundException($"Animal with ID {id} not found for deletion.");
            }
        }
        return Task.CompletedTask;
    }
}