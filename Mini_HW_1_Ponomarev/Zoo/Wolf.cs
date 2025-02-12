namespace Zoo;

public class Wolf : Predator
{
    
    public Wolf(string name, int food, int healthLevel) : base(name, food, healthLevel) {}
    public override string ToString()
    {
        return $"Волк по имени {Name}. Количество потребляемой еды в день: {Food} кг.";
    }
}