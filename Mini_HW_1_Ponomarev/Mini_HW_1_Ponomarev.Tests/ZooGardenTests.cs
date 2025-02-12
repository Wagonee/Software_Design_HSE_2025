namespace Mini_HW_1_Ponomarev.Tests;
using Zoo;
using Xunit;
using NSubstitute;

public class ZooGardenTests
{
    private readonly IVeterinary _veterinary = Substitute.For<IVeterinary>();
    private readonly IZooReporter _reporter = Substitute.For<IZooReporter>();
    [Fact]
    public void AddAnimal_HealthyAnimal_IsAdded()
    {
        _veterinary.IsHealthy(Arg.Any<Animal>()).Returns(true);
        var zoo = new ZooGarden(_veterinary, _reporter);
        var monkey = new Monkey("Test", 5, 85, 7);
        
        zoo.AddAnimal(monkey);
        zoo.ReportAllAnimals();
        
        _reporter.Received().ReportAllAnimals(Arg.Is<List<Animal>>(list => list.Contains(monkey)));
    }
    
    [Fact]
    public void AddAnimal_UnhealthyAnimal_IsNotAdded()
    {
        _veterinary.IsHealthy(Arg.Any<Animal>()).Returns(false);
        var zoo = new ZooGarden(_veterinary, _reporter);
        var monkey = new Monkey("Test", 5, 50, 7);
        
        zoo.AddAnimal(monkey);
        zoo.ReportAllAnimals();
    
        _reporter.Received().ReportAllAnimals(Arg.Is<List<Animal>>(list => list.Count == 0));
    }
    
    [Fact]
    public void AddHerbo_KindHerbo_IsAddedToContactAnimals()
    {
        _veterinary.IsHealthy(Arg.Any<Animal>()).Returns(true);
        var zoo = new ZooGarden(_veterinary, _reporter);
        var rabbit = new Rabbit("Bunny", 3, 78, 7);
        
        zoo.AddAnimal(rabbit);
        zoo.ReportContactAnimals();
        
        _reporter.Received().ReportContactZooAnimals(Arg.Is<List<Herbo>>(list => list.Contains(rabbit)));
    }
    
    [Fact]
    public void AddHerbo_UnkindHerbo_IsNotAddedToContactAnimals()
    {
        _veterinary.IsHealthy(Arg.Any<Animal>()).Returns(true);
        var zoo = new ZooGarden(_veterinary, _reporter);
        var rabbit = new Rabbit("Grumpy", 3, 85, 4);
        
        zoo.AddAnimal(rabbit);
        zoo.ReportContactAnimals();
    
        _reporter.Received().ReportContactZooAnimals(Arg.Is<List<Herbo>>(list => list.Count == 0));
    }
}


