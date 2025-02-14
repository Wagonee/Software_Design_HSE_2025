namespace Zoo;

public abstract class Herbo : Animal
{
    public int KindLevel { get; set; }
    protected Herbo(string name, int food, int healthLevel, int kindness) : base(name, food, healthLevel)
    {
        if (kindness < 0 || kindness > 10)
        {
            throw new ArgumentException("Уровень доброты должен лежать в пределах от 0 до 10.");
        }
        KindLevel = kindness;
    }
}