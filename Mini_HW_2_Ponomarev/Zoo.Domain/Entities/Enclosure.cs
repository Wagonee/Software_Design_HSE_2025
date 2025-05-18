using Zoo.Domain.Events;
using Zoo.Domain.Value_Objects;

namespace Zoo.Domain.Entities;

public class Enclosure
{
    public Guid Id { get; set; }
    public AnimalSpecies Type { get; set; }
    public DateTime LastCleanDate { get; set; }
    
    private readonly List<Guid> _animalIds = new List<Guid>();
    
    public uint Capacity { get; }
    public int QuantityOfAnimal => _animalIds.Count;
    
    public Enclosure(Guid id, AnimalSpecies type, uint capacity)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Enclosure ID cannot be empty.", nameof(id));
        if (capacity == 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
        Id = id;
        Type = type ?? throw new ArgumentNullException(nameof(type)); 
        Capacity = capacity;
        LastCleanDate = DateTime.UtcNow;
    }
    
    public AnimalAddedToEnclosureEvent? AddAnimal(Guid animalId, AnimalSpecies animalSpecies)
    {
        if (QuantityOfAnimal >= Capacity)
        {
            throw new InvalidOperationException($"Enclosure {Id} is full. Cannot add animal {animalId}.");
        }

        if (_animalIds.Contains(animalId))
        {
            return null; 
        }

        if (this.Type.IsPredator != animalSpecies.IsPredator)
        {
            throw new InvalidOperationException($"Cannot add animal {animalId} ({animalSpecies.Type}, IsPredator={animalSpecies.IsPredator}) to enclosure {Id} (Type={this.Type.Type}, IsPredator={this.Type.IsPredator}). Incompatible types.");
        }

        _animalIds.Add(animalId);

        return new AnimalAddedToEnclosureEvent(this.Id, animalId, DateTime.UtcNow);
    }

    public AnimalRemovedFromEnclosureEvent? RemoveAnimal(Guid animalId)
    {
        bool removed = _animalIds.Remove(animalId);

        if (removed)
        {
            return new AnimalRemovedFromEnclosureEvent(this.Id, animalId, DateTime.UtcNow);
        }
        return null; 
    }
    
    public void Clean()
    {
        LastCleanDate = DateTime.UtcNow;
    }
    
    public IEnumerable<Guid> GetAnimalIds() => _animalIds;

}