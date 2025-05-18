using Zoo.Domain.Value_Objects;

namespace Zoo.Application.DTOs;

public record FeedingScheduleDto(
    Guid Id,
    Guid AnimalId,
    string FoodName,
    DateTime ScheduledFeedingTime, 
    DateTime? ActualFeedingTime,  
    FeedingStatus Status
);