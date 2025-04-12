using Zoo.Domain.Value_Objects;

namespace Zoo.Domain.Entities;

public class FeedingSchedule
{
    public Guid Id { get; private set; } 
    public Guid AnimalId { get; private set; } 
    
    public DateTime ActualFeedingTime { get; private set; }
    public DateTime FeedingTime { get; private set; } 
    public Food Food { get; private set; } 
    public FeedingStatus Status { get; private set; }

    public FeedingSchedule(Guid id,Guid animalId, DateTime feedingTime, Food food)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
        }

        if (animalId == Guid.Empty)
        {
            throw new ArgumentException($"'{nameof(animalId)}' cannot be null or empty.", nameof(animalId));
            
        }

        if (feedingTime <= DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(nameof(feedingTime), "Feeding time must be in the future.");
        }
        Id = id;
        AnimalId = animalId;
        FeedingTime = feedingTime;
        Food = food ?? throw new ArgumentNullException(nameof(food));
        Status = FeedingStatus.Scheduled;
    }
    
    public void Reschedule(DateTime newFeedingTime)
    {
        if (Status != FeedingStatus.Scheduled)
        {
            throw new InvalidOperationException("Cannot reschedule a feeding that is not in Scheduled status.");
        }
        if (newFeedingTime <= DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(newFeedingTime), "New feeding time must be in the future.");
        }
        FeedingTime = newFeedingTime;
    }
    
    public void MarkAsCompleted()
    {
        if (Status != FeedingStatus.Scheduled)
        {
            throw new InvalidOperationException("Cannot mark feeding as completed if it's not in Scheduled status.");
        }
        Status = FeedingStatus.Completed;
        ActualFeedingTime = DateTime.Now;
    }
    
    public void MarkAsMissed()
    {
        if (Status != FeedingStatus.Scheduled)
        {
            throw new InvalidOperationException("Cannot mark feeding as missed if it's not in Scheduled status.");
        }
        Status = FeedingStatus.Missed;
    }
    
}