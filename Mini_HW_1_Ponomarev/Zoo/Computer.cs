namespace Zoo;

public class Computer : Thing
{
    public Computer(int number) : base(number) {}
    public override string ToString()
    {
        return $"Инвентарный номер: {Number}. Компьютер.";
    }
}