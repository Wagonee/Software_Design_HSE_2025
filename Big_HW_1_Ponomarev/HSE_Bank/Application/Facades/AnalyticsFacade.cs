using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Interfaces.Repositories;

namespace HSE_Bank.Application.Facades;

public class AnalyticsFacade : IAnalyticsFacade
{
    private readonly IOperationRepository _operationRepository;
    
    public AnalyticsFacade(IOperationRepository operationRepository)
    {
        _operationRepository = operationRepository;
    }

    public decimal GetDifferenceBetweenIncomeAndExpenses()
    {
        var operations = _operationRepository.GetAllOperations();
        var income = operations.Where(o => o.Type == TypeCategory.Income);
        var expenses = operations.Where(o => o.Type == TypeCategory.Expense);
        if (income is not null && expenses is not null)
        {
            return income.Sum(a => a.Amount) - expenses.Sum(a => a.Amount);
        }
        if (income is null && expenses is null)
        {
            return 0;
        }
        if (income is not null && expenses is null)
        {
            return income.Sum(a => a.Amount);
        }
        return -expenses.Sum(a => a.Amount);
    }

    public IEnumerable<IGrouping<TypeCategory, Operation>> GroupOperationsByCategory()
    {
        var operations = _operationRepository.GetAllOperations();
        return operations.GroupBy(o =>
            o.Type == TypeCategory.Income ? TypeCategory.Income :
                TypeCategory.Expense);
    }

}