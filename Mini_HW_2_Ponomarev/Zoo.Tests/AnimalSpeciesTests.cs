using Zoo.Domain.Value_Objects;
using Xunit;

namespace Zoo.Tests
{
    public class AnimalSpeciesTests
    {
        [Theory]
        [InlineData("Lion", true)]
        [InlineData("Zebra", false)]
        public void Constructor_ShouldCreateInstance_WithValidData(string type, bool isPredator)
        {
            var species = new AnimalSpecies(type, isPredator);

            Assert.Equal(type, species.Type);
            Assert.Equal(isPredator, species.IsPredator);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ShouldThrowArgumentException_WhenTypeIsNullOrWhitespace(string invalidType)
        {
            var exception = Assert.Throws<ArgumentException>(() => new AnimalSpecies(invalidType, true));
            Assert.Contains("'type' cannot be null or whitespace.", exception.Message); 
            Assert.Equal("type", exception.ParamName);
        }
    }
}