using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class DeleteOperationCommand : ICommands
{
    private readonly IOperationFacade _operationFacade;
    private readonly int _operationId;

    public DeleteOperationCommand(IOperationFacade operationFacade, int operationId)
    {
        _operationFacade = operationFacade;
        _operationId = operationId;
    }

    public void Execute()
    {
        _operationFacade.DeleteOperation(_operationId);
    }
}