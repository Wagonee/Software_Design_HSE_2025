using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Facades;

public interface IAnalyticsFacade
{ 
    decimal GetDifferenceBetweenIncomeAndExpenses();
    IEnumerable<IGrouping<TypeCategory, Operation>> GroupOperationsByCategory ();
}