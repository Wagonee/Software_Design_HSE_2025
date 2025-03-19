using HSE_Bank.Domain.Interfaces.Exporters;

namespace HSE_Bank.Domain.Entities;
using HSE_Bank.Infrastructure.Exporters;
public class BankAccount
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Balance { get; set; }
    internal BankAccount(string? name, decimal balance, int id)
    {
        Id = id;
        Name = name;
        Balance = balance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount cannot be negative");
        }
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdraw amount cannot be negative");
        }

        if (Balance - amount < 0)
        {
            throw new InvalidOperationException("Deposit amount cannot be negative");
        }
        Balance -= amount;
    }

    public string Accept(IDataExporter visitor) => visitor.ExportBankAccount(this);
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Balance: {Balance}";
    }
}