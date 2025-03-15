using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class CreateAccountCommand : ICommands
{
    private readonly IAccountFacade _accountFacade;
    private readonly decimal _initialBalance;
    private readonly string _accountName;

    public CreateAccountCommand(IAccountFacade accountFacade, decimal initialBalance, string accountName)
    {
        _accountFacade = accountFacade;
        _initialBalance = initialBalance;
        _accountName = accountName;
    }

    public void Execute()
    {
        _accountFacade.CreateAccount(_accountName, _initialBalance);
    }
}