namespace Zoo;

public abstract class Predator : Animal
{
    protected Predator(string name, int food, int healthLevel) : base(name, food, healthLevel) {}
}