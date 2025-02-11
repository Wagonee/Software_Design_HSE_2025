namespace Zoo;

public class Wolf : Predator
{
    
    public Wolf(string name, int food) : base(name, food) {}
    public override string ToString()
    {
        return $"Волк по имени {Name}. Количество потребляемой еды в день: {Food} кг.";
    }
}