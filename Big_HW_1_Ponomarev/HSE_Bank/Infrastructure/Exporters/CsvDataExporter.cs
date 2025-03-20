using HSE_Bank.Domain.Interfaces.Exporters;
using HSE_Bank.Domain.Entities;
using System.Text;
using System.IO;

namespace HSE_Bank.Infrastructure.Exporters
{
    public class CsvDataExporter : IDataExporter
    {
        private static readonly string ExportFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Export");
        private static readonly string AccountFilePath = Path.Combine(ExportFolderPath, "accounts.csv");
        private static readonly string CategoryFilePath = Path.Combine(ExportFolderPath, "categories.csv");
        private static readonly string OperationFilePath = Path.Combine(ExportFolderPath, "operations.csv");

        public CsvDataExporter()
        {
            if (!Directory.Exists(ExportFolderPath))
            {
                Directory.CreateDirectory(ExportFolderPath);
            }
        }

        public void ExportBankAccount(BankAccount account) =>
            SaveToFile(AccountFilePath, $"{account.Id},{account.Name},{account.Balance}");

        public void ExportCategory(Category category) =>
            SaveToFile(CategoryFilePath, $"{category.Id},{category.Name},{category.Type}");

        public void ExportOperation(Operation operation) =>
            SaveToFile(OperationFilePath, $"{operation.Id},{operation.Type},{operation.Amount},{operation.Date},{operation.BankAccountId},{operation.CategoryId}");

        private void SaveToFile(string filePath, string data)
        {
            File.AppendAllText(filePath, data + Environment.NewLine, Encoding.UTF8);
            Console.WriteLine($"Данные добавлены в файл: {filePath}");
        }
    }
}