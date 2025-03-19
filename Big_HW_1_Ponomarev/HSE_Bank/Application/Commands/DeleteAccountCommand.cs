using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class DeleteAccountCommand : ICommands
{
    private readonly IAccountFacade _accountFacade;
    private readonly int _accountId;

    public DeleteAccountCommand(IAccountFacade accountFacade, int accountId)
    {
        _accountFacade = accountFacade;
        _accountId = accountId;
    }
    public void Execute()
    {
        _accountFacade.DeleteAccount(_accountId);
    }
}