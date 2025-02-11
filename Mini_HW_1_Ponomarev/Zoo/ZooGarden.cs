namespace Zoo;

public class ZooGarden
{
    private readonly IVeterinary _veterinary;
    private readonly List<Animal> _animals = new List<Animal>();
    private readonly List<IInventory> _inventory = new List<IInventory>();
    private readonly IZooReporter _reporter;
    public ZooGarden(IVeterinary veterinary, IZooReporter reporter)
    {
        _veterinary = veterinary;
        _reporter = reporter;
    }

    public void AddAnimal(Animal animal)
    {
        if (_animals.Contains(animal)) // Не будем добавлять уже добавленное животное.
        {
            return;
        }
        if (_veterinary.IsHealthy(animal))
        {
            _animals.Add(animal);
        }
    }
    
    public void AddInventory(IInventory inventory)
    {
        if (_inventory.Contains(inventory)) // Добавляем только уникальные предметы.
        {
            return;
        }
        _inventory.Add(inventory);
    }

    public void ReportAllAnimals()
    {
        _reporter.ReportAllAnimals(_animals);
    }
    
    
    public void ReportContactAnimals() => _reporter.ReportContactZooAnimals(_animals.OfType<Herbo>());
    public void ReportInventory()  => _reporter.ReportInventory(_inventory);
    public void ReportConsumptions() => _reporter.ReportTotalConsumption(_animals);
}