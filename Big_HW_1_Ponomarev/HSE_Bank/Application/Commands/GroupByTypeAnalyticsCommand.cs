using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class GroupByTypeAnalyticsCommand : ICommands
{
    private readonly IAnalyticsFacade _analyticsFacade;

    public GroupByTypeAnalyticsCommand(IAnalyticsFacade analyticsFacade)
    {
        _analyticsFacade = analyticsFacade;
    }

    public void Execute()
    {
        var groupedOperations = _analyticsFacade.GroupOperationsByCategory();
        foreach (var group in groupedOperations)
        {
            Console.WriteLine($"\nТип операций: {(group.Key == TypeCategory.Income ? "Доходы" : "Расходы")}");
            foreach (var operation in group)
            {
                Console.WriteLine($"{operation.Date.ToShortDateString()} | {operation.Amount} | {operation.Description}");
            }
        }
    }
}