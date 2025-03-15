using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.IFactories;

namespace HSE_Bank.Infrastructure.Fabrics;

public class DomainObjectFactory : IDomainObjectFactory
{
    public Category CreateCategory(int id, string name, TypeCategory type)
    {
        return new Category(id, name, type);
    }
    public Operation CreateOperation(int id, TypeCategory type, decimal amount, DateTime date, int bankAccountId, int categoryId,
        string? description = null)
    {
        return new Operation(id, type, amount, date, bankAccountId, categoryId, description);
    }
    public BankAccount CreateBankAccount(string name, decimal balance, int id)
    {
        return new BankAccount(name, balance, id);
    }
}