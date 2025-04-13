using Zoo.Application.DTOs;

namespace Zoo.Application.Interfaces
{
    public interface IFeedingService
    {
        Task<FeedingScheduleDto?> GetScheduleByIdAsync(Guid id);
        
        Task<IEnumerable<FeedingScheduleDto>> GetSchedulesByAnimalIdAsync(Guid animalId);
        
        Task<IEnumerable<FeedingScheduleDto>> GetAllSchedulesAsync();
        
        Task<FeedingScheduleDto> AddScheduleAsync(CreateFeedingScheduleDto createDto);
        
        Task<bool> DeleteScheduleAsync(Guid id);
        
        Task RescheduleFeedingAsync(Guid scheduleId, RescheduleFeedingDto rescheduleDto);
        
        Task MarkFeedingCompletedAsync(Guid scheduleId);

        Task MarkFeedingMissedAsync(Guid scheduleId);
    }
}