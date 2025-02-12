using Zoo;
using Xunit;

namespace Mini_HW_1_Ponomarev.Tests;

public class HerboTests
{
    [Fact]
    public void Herbo_InvalidKindness_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Monkey("Test", 5, 100, -1));
        Assert.Throws<ArgumentException>(() => new Monkey("Test", 5, 100, 11));
    }
    [Fact]
    public void Herbo_ValidKindness_CreateInstance()
    {
        var rabbit = new Rabbit("Test", 5, 95, 10);
        Assert.Equal(10, rabbit.KindLevel);
    }
}