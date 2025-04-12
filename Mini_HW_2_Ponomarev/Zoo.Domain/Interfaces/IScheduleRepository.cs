// Mini_HW_2_Ponomarev/Zoo/Domain/Interfaces/IFeedingScheduleRepository.cs
using Zoo.Domain.Entities;

namespace Zoo.Domain.Interfaces
{
    public interface IFeedingScheduleRepository
    {
        Task<FeedingSchedule?> GetFeedingScheduleByIdAsync(Guid scheduleId);

        Task<IEnumerable<FeedingSchedule>> GetAllFeedingSchedulesAsync();

        Task<IEnumerable<FeedingSchedule>> GetFeedingSchedulesByAnimalIdAsync(Guid animalId);

        Task AddAsync(FeedingSchedule schedule);

        Task UpdateAsync(FeedingSchedule schedule);

        Task DeleteAsync(Guid scheduleId);
    }
}