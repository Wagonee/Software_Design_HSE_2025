namespace Zoo;

public class Rabbit : Herbo
{
    public Rabbit(string name, int food, int healthLevel, int kind) : base(name, food, healthLevel, kind) {}
    public override string ToString()
    {
        return $"Кролик по имени {Name}. Количество потребляемой еды в день: {Food} кг, уровень доброты: {KindLevel}.";
    }
}