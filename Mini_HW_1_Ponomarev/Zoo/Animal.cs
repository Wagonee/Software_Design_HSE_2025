namespace Zoo;

public abstract class Animal : IAlive
{ 
    public int Food { get; set; }
    public string? Name { get; }
    
    public int HealthLevel { get; set; }
    protected Animal(string name, int food, int healthLevel)
    {
        if (healthLevel < 0 || healthLevel > 100)
        {
            throw new System.ArgumentException("Показатель здоровья измеряется от 0 до 100");
        }
        Name = name;
        if (food <= 0)
        {
            throw new System.ArgumentException("Кол-во потребляемой еды, должно быть больше 0 кг/день.");
        }
        Food = food;
        HealthLevel = healthLevel;
    }
}