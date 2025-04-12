using Zoo.Domain.Events;
using Zoo.Domain.Value_Objects;

namespace Zoo.Domain.Entities;

public class Animal
{
    
    public Guid Id { get; private set; }
    public Guid? CurrentEnclosureId { get; set; }
    public AnimalSpecies Species { get; }
    public string Name { get; set; }
    public DateTime Birthday { get; }
    public Sex Sex { get; }
    public Food FavouriteFood { get; set; }
    
    public DateTime LastFedTime { get; private set; }
    public HealthStatus HealthStatus { get; set; }

    public Animal(Guid currentEnclosureId, Guid id, AnimalSpecies species, string name, DateTime birthday, Sex sex, Food favouriteFood, DateTime lastFedTime, HealthStatus healthStatus)
    {
        Id = id;
        CurrentEnclosureId = currentEnclosureId;
        Species = species;
        Name = name;
        Birthday = birthday;
        Sex = sex;
        FavouriteFood = favouriteFood;
        LastFedTime = lastFedTime;
        HealthStatus = healthStatus;
    }
    
    public AnimalHealedEvent? Heal()
    {
        if (HealthStatus == HealthStatus.Healthy)
        {
            return null;
        }
        HealthStatus = HealthStatus.Healthy;
        return new AnimalHealedEvent(this.Id, DateTime.UtcNow);
    }

    public AnimalMovedEvent? Move(Guid targetEnclosureId)
    {
        if (HealthStatus == HealthStatus.Unhealthy)
        {
            throw new InvalidOperationException("Cannot move an unhealthy animal.");
        }
        if (targetEnclosureId == Guid.Empty)
        {
            throw new ArgumentException("Target enclosure ID cannot be empty.", nameof(targetEnclosureId));
        }
        if (CurrentEnclosureId == targetEnclosureId)
        {
            return null; 
        }
        
        var previousEnclosureId = CurrentEnclosureId;
        CurrentEnclosureId = targetEnclosureId;

        return new AnimalMovedEvent(this.Id, previousEnclosureId, targetEnclosureId, DateTime.UtcNow);
    }

    public AnimalFedEvent? Feed(DateTime feedingTime)
    {
        LastFedTime = feedingTime;
        return new AnimalFedEvent(this.Id, feedingTime, DateTime.UtcNow);
    }
}