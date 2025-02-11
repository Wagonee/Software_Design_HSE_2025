namespace Zoo;

public interface IZooReporter
{
    void ReportTotalConsumption(IEnumerable<Animal> animals);
    void ReportContactZooAnimals(IEnumerable<Herbo> animals);
    void ReportAllAnimals(IEnumerable<Animal> animals);
    void ReportInventory(IEnumerable<IInventory> inventory);
}