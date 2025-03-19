using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Exporters;

public interface IDataExporter
{
    string ExportBankAccount(BankAccount bankAccount);
    string ExportCategory(Category category);
    string ExportOperation(Operation operation);
}