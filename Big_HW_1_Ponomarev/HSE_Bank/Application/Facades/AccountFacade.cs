using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Interfaces.IFactories;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank.Application.Facades;

public class AccountFacade : IAccountFacade
{
    private readonly IDomainObjectFactory _factory;
    private readonly IAccountRepository _accountRepository;
    

    public AccountFacade(IAccountRepository accountRepository, IDomainObjectFactory factory)
    {
        _accountRepository = accountRepository;
        _factory = factory;
    }
    
    public BankAccount? GetAccountById(int accountId)
    {
        return _accountRepository.GetAccount(accountId);
    }

    public IEnumerable<BankAccount> GetAllAccounts()
    {
        return _accountRepository.GetAllAccounts();
    }

    public BankAccount CreateAccount(string name, decimal balance)
    {
        //TODO: Logic for id creation in work.
        var id = new Random().Next();
        var account = _factory.CreateBankAccount(name, balance, id);
        _accountRepository.AddAccount(account);
        return account;
    }

    public void DeleteAccount(int accountId)
    {
        _accountRepository.DeleteAccount(accountId);
    }

    public void UpdateAccount(int id, string name)
    {
        _accountRepository.UpdateAccount(id, name);
    }
}