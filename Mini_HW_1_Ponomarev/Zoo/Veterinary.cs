namespace Zoo;

public class Veterinary : IVeterinary
{
    public Veterinary() {}
    public bool IsHealthy(Animal animal)
    {
        return animal.HealthLevel >= 75;
    }
}