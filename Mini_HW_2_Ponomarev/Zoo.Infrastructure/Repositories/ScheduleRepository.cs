using Zoo.Domain.Entities;
using Zoo.Domain.Interfaces;
using System.Collections.Concurrent; 

namespace Zoo.Infrastructure.Repositories
{
    public class FeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly ConcurrentDictionary<Guid, FeedingSchedule> _schedules = new();

        public Task<FeedingSchedule?> GetFeedingScheduleByIdAsync(Guid scheduleId)
        {
            _schedules.TryGetValue(scheduleId, out var schedule);
            return Task.FromResult(schedule); 
        }

        public Task<IEnumerable<FeedingSchedule>> GetAllFeedingSchedulesAsync()
        {
            return Task.FromResult<IEnumerable<FeedingSchedule>>(_schedules.Values.ToList());
        }

        public Task<IEnumerable<FeedingSchedule>> GetFeedingSchedulesByAnimalIdAsync(Guid animalId)
        {
            var schedulesForAnimal = _schedules.Values.Where(s => s.AnimalId == animalId).ToList();
            return Task.FromResult<IEnumerable<FeedingSchedule>>(schedulesForAnimal);
        }


        public Task AddAsync(FeedingSchedule schedule)
        {
            if (schedule.Id == Guid.Empty)
            {
                throw new ArgumentException("Schedule ID cannot be empty.", nameof(schedule));
            }

            if (!_schedules.TryAdd(schedule.Id, schedule))
            {
                throw new InvalidOperationException($"Feeding schedule with ID {schedule.Id} already exists.");
            }
            return Task.CompletedTask;
        }

        public Task UpdateAsync(FeedingSchedule schedule)
        {
            if (!_schedules.ContainsKey(schedule.Id))
            {
                throw new KeyNotFoundException($"Feeding schedule with ID {schedule.Id} not found.");
            }
            _schedules[schedule.Id] = schedule;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid scheduleId)
        {
            if (!_schedules.TryRemove(scheduleId, out _))
            {
                throw new KeyNotFoundException($"Feeding schedule with ID {scheduleId} not found for deletion.");
            }
            return Task.CompletedTask;
        }
    }
}