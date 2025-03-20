using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Repositories;

namespace HSE_Bank.Infrastructure.Repositories;

public class InMemoryAccountRepository : IAccountRepository
{
    private readonly List<BankAccount> _accounts = new List<BankAccount>();

    public BankAccount? GetAccount(int id)
    {
        return _accounts.FirstOrDefault(a => a.Id == id);
    }

    public IEnumerable<BankAccount> GetAllAccounts()
    {
        return _accounts;
    }

    public void AddAccount(BankAccount account)
    {
        _accounts.Add(account);
    }

    public void DeleteAccount(int id)
    {
        var accountToDelete = _accounts.FirstOrDefault(a => a.Id == id);
        if (accountToDelete != null)
        {
            _accounts.Remove(accountToDelete);
        }
    }

    public void UpdateAccount(int id, string name)
    {
        int index = _accounts.FindIndex(a => a.Id == id);
        if (index != -1)
        {
            _accounts[index].Name = name;
        }
    }
}