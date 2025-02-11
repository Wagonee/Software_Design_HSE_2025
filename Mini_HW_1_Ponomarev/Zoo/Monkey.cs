namespace Zoo;

public class Monkey : Herbo
{
    public Monkey(string name, int food, int kindness) : base(name, food, kindness) {}
    public override string ToString()
    {
        return $"Обезьяна по имени {Name}. Количество потребляемой еды в день: {Food} кг, уровень доброты: {KindLevel}.";
    }
}