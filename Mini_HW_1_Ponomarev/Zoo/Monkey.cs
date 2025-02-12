namespace Zoo;

public class Monkey : Herbo
{
    public Monkey(string name, int food, int healthLevel, int kind) : base(name, food, healthLevel, kind) {}
    public override string ToString()
    {
        return $"Обезьяна по имени {Name}. Количество потребляемой еды в день: {Food} кг, уровень доброты: {KindLevel}.";
    }
}