namespace Zoo.Application.DTOs;

public record CreateEnclosureDto(
    string SpeciesType,
    bool IsPredatorEnclosure,
    uint Capacity
    );