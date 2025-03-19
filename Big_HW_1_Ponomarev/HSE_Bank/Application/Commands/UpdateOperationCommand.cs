using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class UpdateOperationCommand : ICommands
{
    private readonly IOperationFacade _operationFacade;
    private readonly Operation _operation;

    public UpdateOperationCommand(IOperationFacade operationFacade, Operation operation)
    {
        _operationFacade = operationFacade;
        _operation = operation;
    }

    public void Execute()
    {
        _operationFacade.UpdateOperation(_operation);
    }
}