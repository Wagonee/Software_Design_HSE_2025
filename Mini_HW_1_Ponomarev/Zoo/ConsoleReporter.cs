namespace Zoo;

public class ConsoleReporter : IZooReporter
{
    public void ReportAllAnimals(IEnumerable<Animal> animals)
    {
        Console.WriteLine("Общий список животных в зоопарке сейчас:");
        foreach (var animal in animals)
        {
            Console.WriteLine(animal);
        }
    }
    public void ReportTotalConsumption(IEnumerable<Animal> animals)
    {
        Console.WriteLine($"Общее потребление еды в день: {animals.Sum(item => item.Food)} кг.");
    }

    public void ReportContactZooAnimals(IEnumerable<Herbo> animals)
    {
        Console.WriteLine("Животные, которых можно отправить в контактный зоопарк:");
        foreach (var animal in animals)
        {
            Console.WriteLine(animal);
        }
    }

    public void ReportInventory(IEnumerable<IInventory> inventory)
    {
        Console.WriteLine("Инвентарь зоопарка:");
        foreach (var item in inventory)
        {
            Console.WriteLine(item);
        }
    }
}