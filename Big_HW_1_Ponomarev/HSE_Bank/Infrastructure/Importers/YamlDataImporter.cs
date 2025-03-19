using YamlDotNet.Serialization;

namespace HSE_Bank.Infrastructure.Importers;

public class YamlDataImporter<T> : DataImporter<T>
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder().Build();
    protected override string ReadFile(string filePath) => File.ReadAllText(filePath);
    
    protected override IEnumerable<T> ParseData(string data) =>
        _deserializer.Deserialize<List<T>>(data) ?? new List<T>();
    
    protected override void SaveData(IEnumerable<T> data)
    {
        foreach (var item in data)
        {
            Console.WriteLine($"Imported {typeof(T).Name}: {item}");
        }
    }
}