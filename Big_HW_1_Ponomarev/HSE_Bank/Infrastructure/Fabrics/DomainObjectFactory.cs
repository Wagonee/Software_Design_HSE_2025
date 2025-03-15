using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.IFactories;

namespace HSE_Bank.Infrastructure.Fabrics;

public class DomainObjectFactory : IDomainObjectFactory
{
    public Category CreateCategory(int id, string name, TypeCategory type)
    {
        if (name == null)
        {
            throw new ArgumentException("Category name cannot be null.");
        }
        return new Category(id, name, type);
    }
    public Operation CreateOperation(int id, TypeCategory type, decimal amount, DateTime date, int bankAccountId, int categoryId,
        string? description = null)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.");
        }
        return new Operation(id, type, amount, date, bankAccountId, categoryId, description);
    }
    public BankAccount CreateBankAccount(string name, decimal balance, int id)
    {
        if (balance < 0)
        {
            throw new ArgumentException("Balance amount cannot be negative");
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Bank account name cannot be null or empty.");
        }
        return new BankAccount(name, balance, id);
    }
}