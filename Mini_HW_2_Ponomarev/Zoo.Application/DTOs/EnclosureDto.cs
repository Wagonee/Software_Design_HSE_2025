namespace Zoo.Application.DTOs;

public record EnclosureDto(
    Guid Id,
    string SpeciesType,
    bool IsPredatorEnclosure,
    uint Capacity,
    int CurrentAnimalCount,
    DateTime LastCleanDate,
    List<Guid> AnimalIds
    );