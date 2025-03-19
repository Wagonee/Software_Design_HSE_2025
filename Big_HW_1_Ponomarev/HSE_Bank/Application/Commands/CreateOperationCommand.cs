using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Application.Commands;

public class CreateOperationCommand : ICommands
{
    private readonly IOperationFacade _operationFacade;
    private readonly TypeCategory _type;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly int _bankAccountId;
    private readonly int _categoryId;
    private readonly string? _description;

    public CreateOperationCommand(IOperationFacade operationFacade, TypeCategory type, decimal amount, DateTime date, 
        int bankAccountId, int categoryId, string? description = null)
    {
        _operationFacade = operationFacade;
        _type = type;
        _amount = amount;
        _date = date;
        _bankAccountId = bankAccountId;
        _categoryId = categoryId;
        _description = description;
    }

    public void Execute()
    {
        _operationFacade.CreateOperation(_type, _amount, _date, _bankAccountId, _categoryId, _description);
    }
}