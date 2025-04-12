namespace Zoo.Domain.Events;

public record AnimalHealedEvent(Guid IdGuid, DateTime Date);