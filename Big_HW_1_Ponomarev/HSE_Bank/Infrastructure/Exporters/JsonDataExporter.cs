using HSE_Bank.Domain.Interfaces.Exporters;
using System.Text.Json;
using HSE_Bank.Domain.Entities;
using System.Collections.Generic;

namespace HSE_Bank.Infrastructure.Exporters;

public class JsonDataExporter : IDataExporter
{
    private static readonly string AccountFilePath = "accounts.json";
    private static readonly string CategoryFilePath = "categories.json";
    private static readonly string OperationFilePath = "operations.json";

    public void ExportBankAccount(BankAccount account) => SaveToFile(AccountFilePath, account);
    public void ExportCategory(Category category) => SaveToFile(CategoryFilePath, category);
    public void ExportOperation(Operation operation) => SaveToFile(OperationFilePath, operation);

    private void SaveToFile<T>(string filePath, T data)
    {
        List<T> dataList = new();
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                try
                {
                    dataList = JsonSerializer.Deserialize<List<T>>(existingJson) ?? new List<T>();
                }
                catch
                {
                    dataList = new List<T>(); 
                }
            }
        }

        dataList.Add(data);
        var json = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Данные сохранены в файл: {filePath}");
    }
}