namespace Zoo;

public abstract class Animal : IAlive
{ 
    public int Food { get; set; }
    public string? Name { get; }
    
    protected Animal(string name, int food)
    {
        Name = name;
        if (food <= 0) {throw new System.ArgumentException("Кол-во потребляемой еды, должно быть больше 0 кг/день.");}
        Food = food;
    }
}