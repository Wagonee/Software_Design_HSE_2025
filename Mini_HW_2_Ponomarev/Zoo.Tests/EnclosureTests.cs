using Zoo.Domain.Entities;
using Zoo.Domain.Value_Objects;
using Zoo.Domain.Events;
using Xunit;
namespace Zoo.Tests
{
    public class EnclosureTests
    {
        private readonly AnimalSpecies _predatorSpecies = new("Lion", true);
        private readonly AnimalSpecies _herbivoreSpecies = new("Zebra", false);

        [Fact]
        public void Constructor_ShouldInitializeCorrectly_WithValidData()
        {
            var id = Guid.NewGuid();
            uint capacity = 5;

            var enclosure = new Enclosure(id, _predatorSpecies, capacity);

            Assert.Equal(id, enclosure.Id);
            Assert.Equal(_predatorSpecies, enclosure.Type);
            Assert.Equal(capacity, enclosure.Capacity);
            Assert.Equal(0, enclosure.QuantityOfAnimal);
            Assert.Empty(enclosure.GetAnimalIds());
            Assert.True(enclosure.LastCleanDate <= DateTime.UtcNow && enclosure.LastCleanDate > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() => new Enclosure(Guid.Empty, _predatorSpecies, 5));
            Assert.Equal("id", exception.ParamName);
            Assert.Contains("Enclosure ID cannot be empty.", exception.Message);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenTypeIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Enclosure(Guid.NewGuid(), null!, 5));
            Assert.Equal("type", exception.ParamName);
        }


        [Theory]
        [InlineData(0)]
        public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenCapacityIsZero(uint invalidCapacity)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Enclosure(Guid.NewGuid(), _predatorSpecies, invalidCapacity));
            Assert.Equal("capacity", exception.ParamName);
            Assert.Contains("Capacity must be greater than zero.", exception.Message);
        }

        [Fact]
        public void AddAnimal_ShouldAddAnimalAndReturnEvent_WhenCompatibleAndNotFull()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 2);
            var animalId = Guid.NewGuid();

            var addEvent = enclosure.AddAnimal(animalId, _predatorSpecies);

            Assert.Equal(1, enclosure.QuantityOfAnimal);
            Assert.Contains(animalId, enclosure.GetAnimalIds());
            Assert.NotNull(addEvent);
            Assert.IsType<AnimalAddedToEnclosureEvent>(addEvent);
            Assert.Equal(enclosure.Id, addEvent.EnclosureId);
            Assert.Equal(animalId, addEvent.AnimalId);
        }

        [Fact]
        public void AddAnimal_ShouldThrowInvalidOperationException_WhenEnclosureIsFull()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 1);
            enclosure.AddAnimal(Guid.NewGuid(), _predatorSpecies); 
            var newAnimalId = Guid.NewGuid();

            var exception = Assert.Throws<InvalidOperationException>(() => enclosure.AddAnimal(newAnimalId, _predatorSpecies));
            Assert.Contains($"Enclosure {enclosure.Id} is full. Cannot add animal {newAnimalId}", exception.Message);
        }

        [Fact]
        public void AddAnimal_ShouldReturnNull_WhenAnimalAlreadyExists()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 2);
            var animalId = Guid.NewGuid();
            enclosure.AddAnimal(animalId, _predatorSpecies); 

            var addEvent = enclosure.AddAnimal(animalId, _predatorSpecies); 

            Assert.Equal(1, enclosure.QuantityOfAnimal);
            Assert.Null(addEvent);
        }


        [Fact]
        public void AddAnimal_ShouldThrowInvalidOperationException_WhenSpeciesAreIncompatible()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 2); 
            var herbivoreId = Guid.NewGuid();

            var exception = Assert.Throws<InvalidOperationException>(() => enclosure.AddAnimal(herbivoreId, _herbivoreSpecies)); 
            Assert.Contains("Incompatible types.", exception.Message);

            var enclosure2 = new Enclosure(Guid.NewGuid(), _herbivoreSpecies, 2); 
            var predatorId = Guid.NewGuid();

            var exception2 = Assert.Throws<InvalidOperationException>(() => enclosure2.AddAnimal(predatorId, _predatorSpecies)); 
            Assert.Contains("Incompatible types.", exception2.Message);
        }

        [Fact]
        public void RemoveAnimal_ShouldRemoveAnimalAndReturnEvent_WhenAnimalExists()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 2);
            var animalId = Guid.NewGuid();
            enclosure.AddAnimal(animalId, _predatorSpecies);
            Assert.Equal(1, enclosure.QuantityOfAnimal); 

            var removeEvent = enclosure.RemoveAnimal(animalId);

            Assert.Equal(0, enclosure.QuantityOfAnimal);
            Assert.DoesNotContain(animalId, enclosure.GetAnimalIds());
            Assert.NotNull(removeEvent);
            Assert.IsType<AnimalRemovedFromEnclosureEvent>(removeEvent);
            Assert.Equal(enclosure.Id, removeEvent.EnclosureId);
            Assert.Equal(animalId, removeEvent.AnimalId);
        }

        [Fact]
        public void RemoveAnimal_ShouldReturnNull_WhenAnimalDoesNotExist()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 2);
            var nonExistentAnimalId = Guid.NewGuid();

            var removeEvent = enclosure.RemoveAnimal(nonExistentAnimalId);

            Assert.Equal(0, enclosure.QuantityOfAnimal);
            Assert.Null(removeEvent);
        }

        [Fact]
        public void Clean_ShouldUpdateLastCleanDate()
        {
  
            var enclosure = new Enclosure(Guid.NewGuid(), _predatorSpecies, 2);
            var initialCleanDate = enclosure.LastCleanDate;
            Thread.Sleep(10); 
            enclosure.Clean();

            Assert.NotEqual(initialCleanDate, enclosure.LastCleanDate);
            Assert.True(enclosure.LastCleanDate > initialCleanDate);
            Assert.True(enclosure.LastCleanDate <= DateTime.UtcNow && enclosure.LastCleanDate > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public void GetAnimalIds_ShouldReturnCorrectIds()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), _herbivoreSpecies, 3);
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            enclosure.AddAnimal(id1, _herbivoreSpecies);
            enclosure.AddAnimal(id2, _herbivoreSpecies);
            
            var animalIds = enclosure.GetAnimalIds().ToList();

            Assert.Equal(2, animalIds.Count);
            Assert.Contains(id1, animalIds);
            Assert.Contains(id2, animalIds);
        }
    }
}