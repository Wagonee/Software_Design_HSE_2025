using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Infrastructure.Importers;

public class CsvDataImporter<T> : DataImporter<T>
{
    protected override string ReadFile(string filePath) => File.ReadAllText(filePath);
    
    protected override IEnumerable<T> ParseData(string data)
    {
        var lines = data.Split('\n');
        var result = new List<T>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (typeof(T) == typeof(BankAccount) && parts.Length == 3)
            {
                result.Add((T)(object)new BankAccount(parts[1], decimal.Parse(parts[2]), int.Parse(parts[0])));
            }
            else if (typeof(T) == typeof(Category) && parts.Length == 3)
            {
                result.Add((T)(object)new Category(int.Parse(parts[0]), parts[1], Enum.Parse<TypeCategory>(parts[2])));
            }
            else if (typeof(T) == typeof(Operation) && parts.Length == 6)
            {
                result.Add((T)(object)new Operation(int.Parse(parts[0]), Enum.Parse<TypeCategory>(parts[1]), decimal.Parse(parts[2]), DateTime.Parse(parts[3]), int.Parse(parts[4]), int.Parse(parts[5])));
            }
        }
        return result;
    }

    public override IEnumerable<T> Import(string filepath)
    {
        var strings = File.ReadAllText(filepath);
        var result = ParseData(strings);
        return result;
    }
}