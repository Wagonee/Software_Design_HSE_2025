using System.Runtime.InteropServices.JavaScript;

namespace Zoo;

public class Table : Thing
{
    public Table(int number) : base(number) {}
    public override string ToString()
    {
        return $"Инвентарный номер: {Number}. Стол.";
    }
}