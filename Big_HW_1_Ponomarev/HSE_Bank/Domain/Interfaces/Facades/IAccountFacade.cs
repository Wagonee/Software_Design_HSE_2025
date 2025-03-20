using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Facades;

public interface IAccountFacade
{
    BankAccount? GetAccountById(int accountId);
    IEnumerable<BankAccount> GetAllAccounts();
    BankAccount CreateAccount(string name, decimal balance);
    void DeleteAccount(int accountId);
    void UpdateAccount(int id, string name);
}