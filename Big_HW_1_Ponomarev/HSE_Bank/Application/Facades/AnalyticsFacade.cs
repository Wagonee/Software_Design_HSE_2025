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

    public decimal GetDifferenceBetweenIncomeAndExpenses(DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepository.GetOperationsByPeriod(startDate,
            endDate);
        var income = operations.Where(o => o.Type == TypeCategory.Income)
            .Sum(o => o.Amount);
        var expenses = operations.Where(o => o.Type == TypeCategory.Expense)
            .Sum(o => o.Amount);
        return income - expenses;
    }

    public IEnumerable<IGrouping<TypeCategory, Operation>> GroupOperationsByCategory(DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepository.GetOperationsByPeriod(startDate,
            endDate);
        return operations.GroupBy(o =>
            o.Type == TypeCategory.Income ? TypeCategory.Income :
                TypeCategory.Expense);
    }
}