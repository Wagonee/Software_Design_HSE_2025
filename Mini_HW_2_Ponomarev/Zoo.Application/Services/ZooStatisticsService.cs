using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;
using Zoo.Domain.Interfaces;

namespace Zoo.Application.Services
{
    public class ZooStatisticsService : IZooStatisticsService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;

        public ZooStatisticsService(IAnimalRepository animalRepository, IEnclosureRepository enclosureRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        public async Task<ZooStatisticsDto> GetStatisticsAsync()
        {
            var animalsTask = _animalRepository.GetAllAnimalsAsync();
            var enclosuresTask = _enclosureRepository.GetAllEnclosuresAsync();

            await Task.WhenAll(animalsTask, enclosuresTask);

            var animals = (await animalsTask).ToList(); 
            var enclosures = (await enclosuresTask).ToList();

            var totalAnimals = animals.Count;
            var totalEnclosures = enclosures.Count;

            var emptyEnclosures = enclosures.Count(e => e.QuantityOfAnimal == 0);
            var enclosuresWithFreeSpace = enclosures.Count(e => e.QuantityOfAnimal < e.Capacity);

            var animalsBySpecies = animals
                .GroupBy(a => a.Species.Type) 
                .ToDictionary(g => g.Key, g => g.Count()); 

            var animalsByHealthStatus = animals
                .GroupBy(a => a.HealthStatus) 
                .ToDictionary(g => g.Key, g => g.Count());
            
            var statistics = new ZooStatisticsDto(
                TotalAnimals: totalAnimals,
                TotalEnclosures: totalEnclosures,
                EmptyEnclosures: emptyEnclosures,
                EnclosuresWithFreeSpace: enclosuresWithFreeSpace,
                AnimalsBySpecies: animalsBySpecies,
                AnimalsByHealthStatus: animalsByHealthStatus
            );

            return statistics;
        }
    }
}