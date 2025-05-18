namespace Zoo.Domain.Events;

public record AnimalFedEvent(Guid Guid, DateTime FeedingTime, DateTime OccuredTime);