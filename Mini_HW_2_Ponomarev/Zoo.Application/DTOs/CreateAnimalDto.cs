using Zoo.Domain.Value_Objects;

namespace Zoo.Application.DTOs;

public record CreateAnimalDto(
    string Name,
    string SpeciesType,
    bool IsPredator,
    DateTime Birthday,
    Sex Sex,
    string FavouriteFoodName,
    Guid? InitialEnclosureId
);