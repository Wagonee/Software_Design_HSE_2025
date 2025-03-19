using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Exporters;

namespace HSE_Bank.Infrastructure.Exporters;

public class CsvDataExporter : IDataExporter
{
    public string ExportBankAccount(BankAccount account) =>  $"{account.Id},{account.Name},{account.Balance}";

    public string ExportCategory(Category category) =>  $"{category.Id},{category.Name},{category.Type}";

    public string ExportOperation(Operation operation) =>  $"{operation.Id},{operation.Type},{operation.Amount},{operation.Date},{operation.BankAccountId},{operation.CategoryId}";
}