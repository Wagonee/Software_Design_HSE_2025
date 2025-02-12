using Zoo;
using Xunit;

namespace Mini_HW_1_Ponomarev.Tests;

public class AnimalTests
{
    [Fact]
    public void Animal_InvalidFood_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Monkey("Test", 0, 5, 10));
    }
    
    [Fact]
    public void Animal_InvalidHealth_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Monkey("Test", 2, -1, 10));
    }

    [Fact]
    public void Animal_ValidFood_CreatesInstance()
    {
        var monkey = new Monkey("Test", 5, 5, 10);
        Assert.Equal(5, monkey.Food);
    }
    [Fact]
    public void Animal_ValidHealthLevel_CreatesInstance()
    {
        var monkey = new Monkey("Test", 5, 5, 10);
        Assert.Equal(5, monkey.HealthLevel);
    }
}