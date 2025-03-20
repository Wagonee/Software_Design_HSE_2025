using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class UpdateAccountCommand : ICommands
{
    private readonly IAccountFacade _accountFacade;
    private readonly int _bankAccount;
    private readonly string _bankAccountName;

    public UpdateAccountCommand(IAccountFacade accountFacade, int bankAccount, string name)
    {
        _accountFacade = accountFacade;
        _bankAccount = bankAccount;
        _bankAccountName = name;
    }
    public void Execute()
    {
        _accountFacade.UpdateAccount(_bankAccount, _bankAccountName);
    }
}