using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.IFactories;

public interface IDomainObjectFactory
{
    BankAccount CreateBankAccount(string name, decimal balance, int id);
    Category CreateCategory(int id, string name, TypeCategory type);
    Operation CreateOperation(int id, TypeCategory type, decimal amount, DateTime date,
        int bankAccountId, int categoryId, string? description = null);
}