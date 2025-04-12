namespace Zoo.Domain.Events;

public record AnimalRemovedFromEnclosureEvent(Guid EnclosureId, Guid AnimalId, DateTime OccurredOn);