namespace Zoo;

public class Tiger : Predator
{
    public Tiger(string name, int food, int healthLevel) : base(name, food, healthLevel) {}
    public override string ToString()
    {
        return $"Тигр по имени {Name}. Количество потребляемой еды в день: {Food} кг.";
    }
}