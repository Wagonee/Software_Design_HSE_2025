using Zoo.Domain.Entities;

namespace Zoo.Domain.Interfaces;

public interface IEnclosureRepository
{
    Task<Enclosure>? GetEnclosureByIdAsync(Guid enclosureId);
    Task<IEnumerable<Enclosure>> GetAllEnclosuresAsync();
    Task AddAsync(Enclosure enclosure);
    Task UpdateAsync(Enclosure enclosure);
    Task DeleteAsync(Guid enclosureId);
}