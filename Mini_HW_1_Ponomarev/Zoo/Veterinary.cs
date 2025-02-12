namespace Zoo;

public class Veterinary : IVeterinary
{
    private static readonly Random random = new Random();
    public Veterinary()
    {
    }
    /// <summary>
    /// Проверка на то, что животное здорово (просто добавляем с вероятность 85%).
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public bool IsHealthy(Animal animal)
    {
        return random.NextDouble() < 0.80;
    }
}