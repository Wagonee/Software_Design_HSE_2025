using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.Interfaces;
using Zoo.Domain.Value_Objects; 

namespace Zoo.Application.Services
{
    public class EnclosureService : IEnclosureService
    {
        // УДАЛЕНИЕ НЕПУСТЫХ ВОЛЬЕРОВ ЗАПРЕЩЕНО!!!
        private readonly IEnclosureRepository _enclosureRepository;

        public EnclosureService(IEnclosureRepository enclosureRepository)
        {
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        public async Task<EnclosureDto?> GetEnclosureByIdAsync(Guid id)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByIdAsync(id);
            return enclosure == null ? null : MapEnclosureToDto(enclosure);
        }

        public async Task<IEnumerable<EnclosureDto>> GetAllEnclosuresAsync()
        {
            var enclosures = await _enclosureRepository.GetAllEnclosuresAsync();
            return enclosures.Select(MapEnclosureToDto);
        }

        public async Task<EnclosureDto> AddEnclosureAsync(CreateEnclosureDto createEnclosureDto)
        {
            if (string.IsNullOrWhiteSpace(createEnclosureDto.SpeciesType))
                throw new ArgumentException("Species type cannot be empty.", nameof(createEnclosureDto.SpeciesType));
            if (createEnclosureDto.Capacity == 0)
                 throw new ArgumentOutOfRangeException(nameof(createEnclosureDto.Capacity), "Capacity must be greater than zero.");

            var species = new AnimalSpecies(createEnclosureDto.SpeciesType, createEnclosureDto.IsPredatorEnclosure);

            var newEnclosure = new Enclosure(
                id: Guid.NewGuid(),
                type: species,
                capacity: createEnclosureDto.Capacity
            );

            await _enclosureRepository.AddAsync(newEnclosure);

            return MapEnclosureToDto(newEnclosure);
        }

        public async Task<bool> DeleteEnclosureAsync(Guid id)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByIdAsync(id);
            if (enclosure == null)
            {
                return false; 
            }

            if (enclosure.QuantityOfAnimal > 0)
            {
                throw new InvalidOperationException($"Cannot delete enclosure {id} because it is not empty. Current animal count: {enclosure.QuantityOfAnimal}.");
            }

            await _enclosureRepository.DeleteAsync(id);
            return true;
        }

        public async Task CleanEnclosureAsync(Guid id)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByIdAsync(id);
            if (enclosure == null)
            {
                throw new KeyNotFoundException($"Enclosure with ID {id} not found.");
            }
            
            enclosure.Clean(); 
            await _enclosureRepository.UpdateAsync(enclosure);

            
        }

        private EnclosureDto MapEnclosureToDto(Enclosure enclosure)
        {
            return new EnclosureDto(
                Id: enclosure.Id,
                SpeciesType: enclosure.Type.Type,
                IsPredatorEnclosure: enclosure.Type.IsPredator,
                Capacity: enclosure.Capacity,
                CurrentAnimalCount: enclosure.QuantityOfAnimal,
                LastCleanDate: enclosure.LastCleanDate,
                AnimalIds: enclosure.GetAnimalIds().ToList()
            );
        }
    }
}