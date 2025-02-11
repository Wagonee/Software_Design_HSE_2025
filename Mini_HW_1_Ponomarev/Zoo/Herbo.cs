namespace Zoo;

public class Herbo : Animal
{
    public int KindLevel { get; set; }
    public Herbo(string name, int food, int kindness) : base(name, food)
    {
        KindLevel = kindness;
    }
}