using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Exporters;

public interface IDataExporter
{
    void ExportBankAccount(BankAccount bankAccount);
    void ExportCategory(Category category);
    void ExportOperation(Operation operation);
}