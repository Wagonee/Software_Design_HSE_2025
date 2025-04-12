using Zoo.Domain.Entities;
using Zoo.Domain.Interfaces;

namespace Zoo.Infrastructure.Repositories;

public class EnclosureRepository : IEnclosureRepository
{
    private readonly Dictionary<Guid, Enclosure> _enclosures = new();
    private readonly object _lock = new();
    
    public Task<Enclosure>? GetEnclosureByIdAsync(Guid enclosureId)
    {
        lock (_lock)
        {
            _enclosures.TryGetValue(enclosureId, out var animal);
            return Task.FromResult(animal);
        }
    }

    public Task<IEnumerable<Enclosure>> GetAllEnclosuresAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Enclosure>>(_enclosures.Values.ToList());
        }
    }

    public Task AddAsync(Enclosure enclosure)
    {
        if (enclosure.Id == Guid.Empty)
        {
            if (enclosure.Id == Guid.Empty) throw new ArgumentException("Enclosure ID cannot be empty.", nameof(enclosure));
        }

        lock (_lock)
        {
            if (_enclosures.ContainsKey(enclosure.Id))
            {
                throw new InvalidOperationException($"Enclosure with ID {enclosure.Id} already exists.");
            }
            _enclosures[enclosure.Id] = enclosure;
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Enclosure enclosure)
    {
        lock (_lock)
        {
            if (!_enclosures.ContainsKey(enclosure.Id))
            {
                throw new KeyNotFoundException($"Enclosure with ID {enclosure.Id} not found.");
            }
            _enclosures[enclosure.Id] = enclosure;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid enclosureId)
    {
        lock (_lock)
        {
            if (!_enclosures.Remove(enclosureId))
            {
                throw new KeyNotFoundException($"Enclosure with ID {enclosureId} not found for deletion.");
            }
        }
        return Task.CompletedTask;
    }
}