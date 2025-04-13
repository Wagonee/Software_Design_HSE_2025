using Zoo.Application.DTOs;
using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.Interfaces;
using Zoo.Domain.Value_Objects; 

namespace Zoo.Application.Services
{
    public class FeedingService : IFeedingService
    {
        private readonly IFeedingScheduleRepository _scheduleRepository;
        private readonly IAnimalRepository _animalRepository; 

        public FeedingService(IFeedingScheduleRepository scheduleRepository, IAnimalRepository animalRepository)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }

        public async Task<FeedingScheduleDto?> GetScheduleByIdAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetFeedingScheduleByIdAsync(id);
            return schedule == null ? null : MapScheduleToDto(schedule);
        }

        public async Task<IEnumerable<FeedingScheduleDto>> GetSchedulesByAnimalIdAsync(Guid animalId)
        {
            var schedules = await _scheduleRepository.GetFeedingSchedulesByAnimalIdAsync(animalId);
            return schedules.Select(MapScheduleToDto);
        }

        public async Task<IEnumerable<FeedingScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllFeedingSchedulesAsync();
            return schedules.Select(MapScheduleToDto);
        }

        public async Task<FeedingScheduleDto> AddScheduleAsync(CreateFeedingScheduleDto createDto)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(createDto.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {createDto.AnimalId} not found.");
            }
            if (string.IsNullOrWhiteSpace(createDto.FoodName))
                throw new ArgumentException("Food name cannot be empty.", nameof(createDto.FoodName));

            var food = new Food(createDto.FoodName);
            var newSchedule = new FeedingSchedule(
                id: Guid.NewGuid(),
                animalId: createDto.AnimalId,
                feedingTime: createDto.ScheduledFeedingTime,
                food: food
            );

            await _scheduleRepository.AddAsync(newSchedule);
            return MapScheduleToDto(newSchedule);
        }

        public async Task<bool> DeleteScheduleAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetFeedingScheduleByIdAsync(id);
            if (schedule == null)
            {
                return false;
            }
            await _scheduleRepository.DeleteAsync(id);
            return true;
        }

        public async Task RescheduleFeedingAsync(Guid scheduleId, RescheduleFeedingDto rescheduleDto)
        {
            var schedule = await _scheduleRepository.GetFeedingScheduleByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Feeding schedule with ID {scheduleId} not found.");
            }

            schedule.Reschedule(rescheduleDto.NewFeedingTime);

            await _scheduleRepository.UpdateAsync(schedule);
        }

        public async Task MarkFeedingCompletedAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepository.GetFeedingScheduleByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Feeding schedule with ID {scheduleId} not found.");
            }

            var animal = await _animalRepository.GetAnimalByIdAsync(schedule.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {schedule.AnimalId} associated with schedule {scheduleId} not found.");
            }

            schedule.MarkAsCompleted(); 
            animal.Feed(schedule.ActualFeedingTime); 

            await _scheduleRepository.UpdateAsync(schedule);
            await _animalRepository.UpdateAsync(animal);
        }

        public async Task MarkFeedingMissedAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepository.GetFeedingScheduleByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Feeding schedule with ID {scheduleId} not found.");
            }

            schedule.MarkAsMissed(); 

            await _scheduleRepository.UpdateAsync(schedule);
        }

        private FeedingScheduleDto MapScheduleToDto(FeedingSchedule schedule)
        {
            return new FeedingScheduleDto(
                Id: schedule.Id,
                AnimalId: schedule.AnimalId,
                FoodName: schedule.Food.Name,
                ScheduledFeedingTime: schedule.FeedingTime, 
                ActualFeedingTime: schedule.Status == FeedingStatus.Completed ? schedule.ActualFeedingTime : null,
                Status: schedule.Status
            );
        }
    }
}