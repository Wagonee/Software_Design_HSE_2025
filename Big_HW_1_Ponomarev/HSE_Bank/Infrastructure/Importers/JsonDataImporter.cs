using System.Text.Json;

namespace HSE_Bank.Infrastructure.Importers;

public class JsonDataImporter<T> : DataImporter<T>
{
    protected override string ReadFile(string filePath) => File.ReadAllText(filePath);

    protected override IEnumerable<T> ParseData(string data) =>
        JsonSerializer.Deserialize<List<T>>(data) ?? new List<T>();

    protected override void SaveData(IEnumerable<T> data)
    {
        foreach (var item in data)
        {
            Console.WriteLine($"Imported {typeof(T).Name}: {item}");
        }
    }
}