using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Repositories;

public interface IAccountRepository
{
    BankAccount? GetAccount(int id);
    IEnumerable<BankAccount> GetAllAccounts();
    void AddAccount(BankAccount account);
    void UpdateAccount(int id, string name);
    void DeleteAccount(int id);
}