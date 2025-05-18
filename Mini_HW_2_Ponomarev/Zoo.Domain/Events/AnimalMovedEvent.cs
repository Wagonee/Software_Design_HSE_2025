namespace Zoo.Domain.Events
{
    public record AnimalMovedEvent(
        Guid AnimalId,
        Guid? PreviousEnclosureId,
        Guid TargetEnclosureId,
        DateTime OccurredOn 
    );
}