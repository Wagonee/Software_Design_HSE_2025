using Zoo.Application.DTOs;

namespace Zoo.Application.Interfaces
{
    public interface IEnclosureService
    {
        Task<EnclosureDto?> GetEnclosureByIdAsync(Guid id);
        
        Task<IEnumerable<EnclosureDto>> GetAllEnclosuresAsync();
        
        Task<EnclosureDto> AddEnclosureAsync(CreateEnclosureDto createEnclosureDto);
        
        Task<bool> DeleteEnclosureAsync(Guid id);

        Task CleanEnclosureAsync(Guid id);
    }
}