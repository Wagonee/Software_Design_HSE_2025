using Zoo.Domain.Value_Objects;
using Xunit;

namespace Zoo.Tests
{
    public class FoodTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance_WithValidName()
        {
            var validName = "Meat";

            var food = new Food(validName);

            Assert.Equal(validName, food.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ShouldThrowArgumentException_WhenNameIsNullOrWhitespace(string invalidName)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Food(invalidName));
            Assert.Contains("Food name cannot be empty.", exception.Message);
            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void FoodRecord_ShouldHaveValueEquality()
        {
            var food1 = new Food("Meat");
            var food2 = new Food("Meat");
            var food3 = new Food("Fish");

            Assert.Equal(food1, food2);       
            Assert.NotEqual(food1, food3);    
            Assert.True(food1 == food2);      
            Assert.False(food1 == food3);     
            Assert.Equal(food1.GetHashCode(), food2.GetHashCode()); 
        }
    }
}