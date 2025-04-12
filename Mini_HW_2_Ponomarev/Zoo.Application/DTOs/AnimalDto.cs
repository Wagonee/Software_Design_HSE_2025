using Zoo.Domain.Value_Objects; 

namespace Zoo.Application.DTOs
{
    public record AnimalDto(
        Guid Id,
        string Name,
        string SpeciesType,
        bool IsPredator,
        DateTime Birthday,
        Sex Sex,
        string FavouriteFoodName,
        HealthStatus HealthStatus,
        Guid? CurrentEnclosureId,
        DateTime LastFedTime
    );
}