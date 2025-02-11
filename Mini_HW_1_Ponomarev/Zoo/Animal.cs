namespace Zoo;

public abstract class Animal : IAlive
{ 
    public int Food { get; set; }
    public string? Name { get; }
    
    protected Animal(string name, int food)
    {
        Name = name;
        Food = food;
    }
}