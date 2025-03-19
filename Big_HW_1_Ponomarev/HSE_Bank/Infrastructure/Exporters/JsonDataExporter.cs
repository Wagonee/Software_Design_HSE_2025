using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Exporters;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace HSE_Bank.Infrastructure.Exporters;

public class JsonDataExporter : IDataExporter
{
    public string ExportBankAccount(BankAccount bankAccount) => JsonSerializer.Serialize(bankAccount);
    public string ExportCategory(Category category) => JsonSerializer.Serialize(category);
    public string ExportOperation(Operation operation) => JsonSerializer.Serialize(operation);
}