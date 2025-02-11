namespace Zoo;

public abstract class Thing : IInventory
{
    protected Thing(int number) => Number = number;
    public int Number { get; set; }
}