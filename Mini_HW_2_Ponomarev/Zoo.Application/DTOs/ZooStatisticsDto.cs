using Zoo.Domain.Value_Objects;

namespace Zoo.Application.DTOs
{
    public record ZooStatisticsDto(
        int TotalAnimals,
        int TotalEnclosures,
        int EmptyEnclosures, 
        int EnclosuresWithFreeSpace,
        Dictionary<string, int> AnimalsBySpecies, 
        Dictionary<HealthStatus, int> AnimalsByHealthStatus 
    );
}