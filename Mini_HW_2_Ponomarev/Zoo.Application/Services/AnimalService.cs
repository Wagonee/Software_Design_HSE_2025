using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.Interfaces;
using Zoo.Domain.Value_Objects; 

namespace Zoo.Application.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository; 

        public AnimalService(IAnimalRepository animalRepository, IEnclosureRepository enclosureRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        public async Task<AnimalDto?> GetAnimalByIdAsync(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            return animal == null ? null : MapAnimalToDto(animal); 
        }

        public async Task<IEnumerable<AnimalDto>> GetAllAnimalsAsync()
        {
            var animals = await _animalRepository.GetAllAnimalsAsync();
            return animals.Select(MapAnimalToDto);
        }

        public async Task<AnimalDto> AddAnimalAsync(CreateAnimalDto createAnimalDto)
        {
            if (string.IsNullOrWhiteSpace(createAnimalDto.Name))
                throw new ArgumentException("Animal name cannot be empty.", nameof(createAnimalDto.Name));
            if (string.IsNullOrWhiteSpace(createAnimalDto.SpeciesType))
                throw new ArgumentException("Species type cannot be empty.", nameof(createAnimalDto.SpeciesType));
            if (string.IsNullOrWhiteSpace(createAnimalDto.FavouriteFoodName))
                throw new ArgumentException("Favourite food name cannot be empty.", nameof(createAnimalDto.FavouriteFoodName));
            var species = new AnimalSpecies(createAnimalDto.SpeciesType, createAnimalDto.IsPredator);
            var favouriteFood = new Food(createAnimalDto.FavouriteFoodName);
            var newAnimal = new Animal(
                id: Guid.NewGuid(), 
                currentEnclosureId: Guid.NewGuid(), 
                species: species,
                name: createAnimalDto.Name,
                birthday: createAnimalDto.Birthday,
                sex: createAnimalDto.Sex,
                favouriteFood: favouriteFood,
                lastFedTime: DateTime.UtcNow, 
                healthStatus: HealthStatus.Healthy 
            );

            Enclosure? initialEnclosure = null;
            if (createAnimalDto.InitialEnclosureId.HasValue && createAnimalDto.InitialEnclosureId.Value != Guid.Empty)
            {
                initialEnclosure = await _enclosureRepository.GetEnclosureByIdAsync(createAnimalDto.InitialEnclosureId.Value);
                if (initialEnclosure == null)
                {
                    throw new KeyNotFoundException($"Initial enclosure with ID {createAnimalDto.InitialEnclosureId.Value} not found.");
                }
                
                initialEnclosure.AddAnimal(newAnimal.Id, newAnimal.Species);
                newAnimal.CurrentEnclosureId = initialEnclosure.Id;
            }

            await _animalRepository.AddAsync(newAnimal);
            if (initialEnclosure != null)
            {
                await _enclosureRepository.UpdateAsync(initialEnclosure);
            }
            return MapAnimalToDto(newAnimal);
        }

        public async Task<bool> DeleteAnimalAsync(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return false; 
            }

            if (animal.CurrentEnclosureId.HasValue)
            {
                var currentEnclosure = await _enclosureRepository.GetEnclosureByIdAsync(animal.CurrentEnclosureId.Value);
                if (currentEnclosure != null) 
                {
                    currentEnclosure.RemoveAnimal(id);
                    await _enclosureRepository.UpdateAsync(currentEnclosure); 
                }
            }

            await _animalRepository.DeleteAsync(id);
            return true;
        }

        public async Task MoveAnimalAsync(Guid animalId, Guid targetEnclosureId)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(animalId);
            if (animal == null) throw new KeyNotFoundException($"Animal with ID {animalId} not found.");

            var targetEnclosure = await _enclosureRepository.GetEnclosureByIdAsync(targetEnclosureId);
            if (targetEnclosure == null) throw new KeyNotFoundException($"Target enclosure with ID {targetEnclosureId} not found.");

            Enclosure? currentEnclosure = null;
            if (animal.CurrentEnclosureId.HasValue)
            {
                if (animal.CurrentEnclosureId.Value == targetEnclosureId)
                {
                    return;
                }
                currentEnclosure = await _enclosureRepository.GetEnclosureByIdAsync(animal.CurrentEnclosureId.Value);
            }

            targetEnclosure.AddAnimal(animalId, animal.Species); 

            animal.Move(targetEnclosureId); 
            currentEnclosure?.RemoveAnimal(animalId);
            await _animalRepository.UpdateAsync(animal);
            await _enclosureRepository.UpdateAsync(targetEnclosure);
            if (currentEnclosure != null)
            {
                await _enclosureRepository.UpdateAsync(currentEnclosure);
            }
        }


        public async Task FeedAnimalAsync(Guid animalId, DateTime feedingTime)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(animalId);
            if (animal == null) throw new KeyNotFoundException($"Animal with ID {animalId} not found.");

            animal.Feed(feedingTime); 

            await _animalRepository.UpdateAsync(animal); 
        }

        public async Task HealAnimalAsync(Guid animalId)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(animalId);
            if (animal == null) throw new KeyNotFoundException($"Animal with ID {animalId} not found.");

            var healEvent = animal.Heal(); 

            if (healEvent != null) 
            {
                await _animalRepository.UpdateAsync(animal); 
            }
        }


        private AnimalDto MapAnimalToDto(Animal animal)
        {
            return new AnimalDto(
                Id: animal.Id,
                Name: animal.Name,
                SpeciesType: animal.Species.Type,
                IsPredator: animal.Species.IsPredator,
                Birthday: animal.Birthday,
                Sex: animal.Sex,
                FavouriteFoodName: animal.FavouriteFood.Name,
                HealthStatus: animal.HealthStatus,
                CurrentEnclosureId: animal.CurrentEnclosureId,
                LastFedTime: animal.LastFedTime
            );
        }
    }
}