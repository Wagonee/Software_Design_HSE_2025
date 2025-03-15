using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Facades;

public interface IAnalyticsFacade
{
    decimal GetDifferenceBetweenIncomeAndExpenses(DateTime startDate, DateTime endDate);
    IEnumerable<IGrouping<TypeCategory, Operation>> GroupOperationsByCategory (DateTime startDate, DateTime endDate);
}