using Zoo.Domain.Entities;
using Zoo.Domain.Value_Objects;
using Xunit;

namespace Zoo.Tests
{
    public class FeedingScheduleTests
    {
        private readonly Food _testFood = new("Test Food");
        private readonly Guid _validAnimalId = Guid.NewGuid();
        private readonly Guid _validScheduleId = Guid.NewGuid();

        private DateTime GetFutureLocalTime()
        {
            return DateTime.Now.AddMinutes(5); 
        }

        private DateTime GetFutureUtcTime()
        {
            return DateTime.UtcNow.AddMinutes(5); 
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly_WhenDataIsValid()
        {
            var futureLocalTime = GetFutureLocalTime(); 
            var scheduleId = Guid.NewGuid();
            var animalId = Guid.NewGuid();

            var schedule = new FeedingSchedule(scheduleId, animalId, futureLocalTime, _testFood);

            Assert.Equal(scheduleId, schedule.Id);
            Assert.Equal(animalId, schedule.AnimalId);
            Assert.Equal(futureLocalTime, schedule.FeedingTime); 
            Assert.Equal(_testFood, schedule.Food);
            Assert.Equal(FeedingStatus.Scheduled, schedule.Status);
            Assert.Equal(default(DateTime), schedule.ActualFeedingTime);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            var futureLocalTime = GetFutureLocalTime();
            var ex = Assert.Throws<ArgumentException>(
                () => new FeedingSchedule(Guid.Empty, _validAnimalId, futureLocalTime, _testFood)
            );
            Assert.Equal("id", ex.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenAnimalIdIsEmpty()
        {
            var futureLocalTime = GetFutureLocalTime();
            var ex = Assert.Throws<ArgumentException>(
                () => new FeedingSchedule(_validScheduleId, Guid.Empty, futureLocalTime, _testFood)
            );
            Assert.Equal("animalId", ex.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenFoodIsNull()
        {
            var futureLocalTime = GetFutureLocalTime();
            var ex = Assert.Throws<ArgumentNullException>(
                () => new FeedingSchedule(_validScheduleId, _validAnimalId, futureLocalTime, null!)
            );
            Assert.Equal("food", ex.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenFeedingTimeIsInThePast()
        {
            var pastLocalTime = DateTime.Now.AddMinutes(-5); 

            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => new FeedingSchedule(_validScheduleId, _validAnimalId, pastLocalTime, _testFood)
            );
            Assert.Equal("feedingTime", ex.ParamName);
            Assert.Contains("Feeding time must be in the future.", ex.Message);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenFeedingTimeIsNow()
        {
            var nowLocalTime = DateTime.Now; 
            Thread.Sleep(10);
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => new FeedingSchedule(_validScheduleId, _validAnimalId, nowLocalTime, _testFood)
            );
            Assert.Equal("feedingTime", ex.ParamName);
            Assert.Contains("Feeding time must be in the future.", ex.Message);
        }

        [Fact]
        public void Reschedule_ShouldUpdateFeedingTime_WhenStatusIsScheduledAndNewTimeIsValidUtc()
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            var newFutureUtcTime = GetFutureUtcTime(); 

            schedule.Reschedule(newFutureUtcTime);

            Assert.Equal(newFutureUtcTime, schedule.FeedingTime); 
            Assert.Equal(FeedingStatus.Scheduled, schedule.Status);
        }

        [Fact]
        public void Reschedule_ShouldThrowArgumentOutOfRangeException_WhenNewTimeIsInPastUtc()
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            var pastUtcTime = DateTime.UtcNow.AddMinutes(-5); 

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => schedule.Reschedule(pastUtcTime));
            Assert.Equal("newFeedingTime", ex.ParamName);
            Assert.Contains("New feeding time must be in the future.", ex.Message);
        }

        [Fact]
        public void Reschedule_ShouldThrowArgumentOutOfRangeException_WhenNewTimeIsNowUtc()
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            var nowUtcTime = DateTime.UtcNow; 
            Thread.Sleep(10); 
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => schedule.Reschedule(nowUtcTime));
            Assert.Equal("newFeedingTime", ex.ParamName);
            Assert.Contains("New feeding time must be in the future.", ex.Message);
        }

        [Theory]
        [InlineData(FeedingStatus.Completed)]
        [InlineData(FeedingStatus.Missed)]
        public void Reschedule_ShouldThrowInvalidOperationException_WhenStatusIsNotScheduled(FeedingStatus initialStatus)
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            if (initialStatus == FeedingStatus.Completed) schedule.MarkAsCompleted();
            else if (initialStatus == FeedingStatus.Missed) schedule.MarkAsMissed();

            var newValidUtcTime = GetFutureUtcTime();
            var ex = Assert.Throws<InvalidOperationException>(() => schedule.Reschedule(newValidUtcTime));
            Assert.Contains("Cannot reschedule a feeding that is not in Scheduled status.", ex.Message);
        }

        [Fact]
        public void MarkAsCompleted_ShouldUpdateStatusAndActualLocalTime_WhenStatusIsScheduled()
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            var timeBeforeActLocal = DateTime.Now; 
            Thread.Sleep(10); 

            schedule.MarkAsCompleted();
            Thread.Sleep(10); 
            var timeAfterActLocal = DateTime.Now;  

            Assert.Equal(FeedingStatus.Completed, schedule.Status);
            Assert.True(schedule.ActualFeedingTime >= timeBeforeActLocal && schedule.ActualFeedingTime <= timeAfterActLocal);
            Assert.NotEqual(DateTimeKind.Utc, schedule.ActualFeedingTime.Kind);
        }

        [Theory]
        [InlineData(FeedingStatus.Completed)]
        [InlineData(FeedingStatus.Missed)]
        public void MarkAsCompleted_ShouldThrowInvalidOperationException_WhenStatusIsNotScheduled(FeedingStatus initialStatus)
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            if (initialStatus == FeedingStatus.Completed) schedule.MarkAsCompleted();
            else if (initialStatus == FeedingStatus.Missed) schedule.MarkAsMissed();

            var ex = Assert.Throws<InvalidOperationException>(() => schedule.MarkAsCompleted());
            Assert.Contains("Cannot mark feeding as completed if it's not in Scheduled status.", ex.Message);
        }

    
        [Fact]
        public void MarkAsMissed_ShouldUpdateStatus_WhenStatusIsScheduled()
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);

            schedule.MarkAsMissed();

            Assert.Equal(FeedingStatus.Missed, schedule.Status);
            Assert.Equal(default(DateTime), schedule.ActualFeedingTime);
        }

        [Theory]
        [InlineData(FeedingStatus.Completed)]
        [InlineData(FeedingStatus.Missed)]
        public void MarkAsMissed_ShouldThrowInvalidOperationException_WhenStatusIsNotScheduled(FeedingStatus initialStatus)
        {
            var schedule = new FeedingSchedule(_validScheduleId, _validAnimalId, GetFutureLocalTime(), _testFood);
            if (initialStatus == FeedingStatus.Completed) schedule.MarkAsCompleted();
            else if (initialStatus == FeedingStatus.Missed) schedule.MarkAsMissed();

            var ex = Assert.Throws<InvalidOperationException>(() => schedule.MarkAsMissed());
            Assert.Contains("Cannot mark feeding as missed if it's not in Scheduled status.", ex.Message);
        }
    }
}