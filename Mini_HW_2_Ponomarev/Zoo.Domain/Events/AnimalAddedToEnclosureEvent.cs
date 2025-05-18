namespace Zoo.Domain.Events;

public record AnimalAddedToEnclosureEvent(Guid EnclosureId, Guid AnimalId, DateTime OccurredOn);