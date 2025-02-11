namespace Zoo;

public class Rabbit : Herbo
{
    public Rabbit(string name, int health, int kind) : base(name, health, kind) {}
    public override string ToString()
    {
        return $"Кролик по имени {Name}. Количество потребляемой еды в день: {Food} кг, уровень доброты: {KindLevel}.";
    }
}