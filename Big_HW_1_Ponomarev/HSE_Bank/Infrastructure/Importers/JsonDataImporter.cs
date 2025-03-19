using System.Text.Json;
using System.Collections.Generic;

namespace HSE_Bank.Infrastructure.Importers;

public class JsonDataImporter<T> : DataImporter<T>
{
    protected override string ReadFile(string filePath) => File.ReadAllText(filePath);

    protected override IEnumerable<T> ParseData(string data)
    {
        try
        {
            return JsonSerializer.Deserialize<List<T>>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при разборе JSON в файле: {ex.Message}");
            return new List<T>();
        }
    }

    public override IEnumerable<T> Import(string filepath)
    {
        if (!File.Exists(filepath)) return new List<T>();

        string fileContent = ReadFile(filepath);
        return ParseData(fileContent);
    }
}