using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class UpdateAccountCommand : ICommands
{
    private readonly IAccountFacade _accountFacade;
    private readonly BankAccount _bankAccount;

    public UpdateAccountCommand(IAccountFacade accountFacade, BankAccount bankAccount)
    {
        _accountFacade = accountFacade;
        _bankAccount = bankAccount;
    }
    public void Execute()
    {
        _accountFacade.UpdateAccount(_bankAccount);
    }
}