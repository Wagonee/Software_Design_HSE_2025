using Zoo.Domain.Entities;
using Zoo.Domain.Value_Objects;
using Zoo.Domain.Events;
using Xunit;

namespace Zoo.Tests
{
    public class AnimalTests
    {
        private readonly AnimalSpecies _lionSpecies = new("Lion", true);
        private readonly AnimalSpecies _zebraSpecies = new("Zebra", false);
        private readonly Food _meat = new("Meat");
        private readonly Food _grass = new("Grass");

        private Animal CreateTestAnimal(
            Guid? id = null,
            Guid? enclosureId = null,
            AnimalSpecies? species = null,
            string name = "TestAnimal",
            DateTime? birthday = null,
            Sex sex = Sex.Male,
            Food? favouriteFood = null,
            DateTime? lastFedTime = null,
            HealthStatus healthStatus = HealthStatus.Healthy)
        {
            return new Animal(
                id: id ?? Guid.NewGuid(),
                currentEnclosureId: enclosureId ?? Guid.NewGuid(),
                species: species ?? _lionSpecies,
                name: name,
                birthday: birthday ?? DateTime.UtcNow.AddYears(-2),
                sex: sex,
                favouriteFood: favouriteFood ?? _meat,
                lastFedTime: lastFedTime ?? DateTime.UtcNow.AddHours(-5),
                healthStatus: healthStatus
            );
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            var id = Guid.NewGuid();
            var enclosureId = Guid.NewGuid();
            var birthday = DateTime.UtcNow.AddYears(-1);
            var lastFed = DateTime.UtcNow.AddHours(-2);

            var animal = new Animal(enclosureId, id, _lionSpecies, "Simba", birthday, Sex.Male, _meat, lastFed, HealthStatus.Healthy);

            Assert.Equal(id, animal.Id);
            Assert.Equal(enclosureId, animal.CurrentEnclosureId);
            Assert.Equal(_lionSpecies, animal.Species);
            Assert.Equal("Simba", animal.Name);
            Assert.Equal(birthday, animal.Birthday);
            Assert.Equal(Sex.Male, animal.Sex);
            Assert.Equal(_meat, animal.FavouriteFood);
            Assert.Equal(lastFed, animal.LastFedTime);
            Assert.Equal(HealthStatus.Healthy, animal.HealthStatus);
        }

        [Fact]
        public void Heal_ShouldChangeStatusToHealthyAndReturnEvent_WhenUnhealthy()
        {
            var animal = CreateTestAnimal(healthStatus: HealthStatus.Unhealthy);
            var initialStatus = animal.HealthStatus;

            var healEvent = animal.Heal();

            Assert.Equal(HealthStatus.Unhealthy, initialStatus);
            Assert.Equal(HealthStatus.Healthy, animal.HealthStatus);
            Assert.NotNull(healEvent);
            Assert.IsType<AnimalHealedEvent>(healEvent);
            Assert.Equal(animal.Id, healEvent.IdGuid);
            Assert.True(healEvent.Date <= DateTime.UtcNow && healEvent.Date > DateTime.UtcNow.AddSeconds(-5)); // Check time
        }

        [Fact]
        public void Heal_ShouldDoNothingAndReturnNull_WhenAlreadyHealthy()
        {
            var animal = CreateTestAnimal(healthStatus: HealthStatus.Healthy);

            var healEvent = animal.Heal();

            Assert.Equal(HealthStatus.Healthy, animal.HealthStatus);
            Assert.Null(healEvent);
        }

        [Fact]
        public void Move_ShouldUpdateEnclosureIdAndReturnEvent_WhenHealthyAndDifferentEnclosure()
        {
            var animal = CreateTestAnimal(healthStatus: HealthStatus.Healthy);
            var originalEnclosureId = animal.CurrentEnclosureId;
            var targetEnclosureId = Guid.NewGuid();

            var moveEvent = animal.Move(targetEnclosureId);

            Assert.Equal(targetEnclosureId, animal.CurrentEnclosureId);
            Assert.NotNull(moveEvent);
            Assert.IsType<AnimalMovedEvent>(moveEvent);
            Assert.Equal(animal.Id, moveEvent.AnimalId);
            Assert.Equal(originalEnclosureId, moveEvent.PreviousEnclosureId);
            Assert.Equal(targetEnclosureId, moveEvent.TargetEnclosureId);
            Assert.True(moveEvent.OccurredOn <= DateTime.UtcNow && moveEvent.OccurredOn > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public void Move_ShouldReturnNull_WhenTargetIsSameEnclosure()
        {
            var enclosureId = Guid.NewGuid();
            var animal = CreateTestAnimal(enclosureId: enclosureId, healthStatus: HealthStatus.Healthy);
            var moveEvent = animal.Move(enclosureId);

            Assert.Equal(enclosureId, animal.CurrentEnclosureId); 
            Assert.Null(moveEvent);
        }


        [Fact]
        public void Move_ShouldThrowInvalidOperationException_WhenUnhealthy()
        {
            var animal = CreateTestAnimal(healthStatus: HealthStatus.Unhealthy);
            var targetEnclosureId = Guid.NewGuid();

            Assert.Throws<InvalidOperationException>(() => animal.Move(targetEnclosureId));
        }

        [Fact]
        public void Move_ShouldThrowArgumentException_WhenTargetEnclosureIdIsEmpty()
        {
            var animal = CreateTestAnimal(healthStatus: HealthStatus.Healthy);

            var exception = Assert.Throws<ArgumentException>(() => animal.Move(Guid.Empty));
            Assert.Equal("targetEnclosureId", exception.ParamName);
            Assert.Contains("Target enclosure ID cannot be empty.", exception.Message);
        }

        [Fact]
        public void Feed_ShouldUpdateLastFedTimeAndReturnEvent()
        {
            var animal = CreateTestAnimal();
            var initialFeedTime = animal.LastFedTime;
            var newFeedTime = DateTime.UtcNow;

            var feedEvent = animal.Feed(newFeedTime);

            Assert.NotEqual(initialFeedTime, animal.LastFedTime);
            Assert.Equal(newFeedTime, animal.LastFedTime);
            Assert.NotNull(feedEvent);
            Assert.IsType<AnimalFedEvent>(feedEvent);
            Assert.Equal(animal.Id, feedEvent.Guid);
            Assert.Equal(newFeedTime, feedEvent.FeedingTime);
            Assert.True(feedEvent.OccuredTime <= DateTime.UtcNow && feedEvent.OccuredTime > DateTime.UtcNow.AddSeconds(-5));
        }
    }
}