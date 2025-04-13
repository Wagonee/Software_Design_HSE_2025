namespace Zoo.Application.DTOs;

public record CreateFeedingScheduleDto(
    Guid AnimalId,
    DateTime ScheduledFeedingTime, 
    string FoodName
);