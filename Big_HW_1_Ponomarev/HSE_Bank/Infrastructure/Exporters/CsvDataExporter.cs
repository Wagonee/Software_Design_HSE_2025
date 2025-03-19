using HSE_Bank.Domain.Interfaces.Exporters;
using HSE_Bank.Domain.Entities;
using System.Text;

namespace HSE_Bank.Infrastructure.Exporters;

public class CsvDataExporter : IDataExporter
{
    public void ExportBankAccount(BankAccount account) => SaveToFile("accounts.csv", $"{account.Id},{account.Name},{account.Balance}");
    public void ExportCategory(Category category) => SaveToFile("categories.csv", $"{category.Id},{category.Name},{category.Type}");
    public void ExportOperation(Operation operation) => SaveToFile("operations.csv", $"{operation.Id},{operation.Type},{operation.Amount},{operation.Date},{operation.BankAccountId},{operation.CategoryId}");

    private void SaveToFile(string filePath, string data)
    {
        File.AppendAllText(filePath, data + Environment.NewLine, Encoding.UTF8);
        Console.WriteLine($"Данные добавлены в файл: {filePath}");
    }
}