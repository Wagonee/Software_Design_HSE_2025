using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class DifferenceAnalyticsCommand : ICommands
{
    private readonly IAnalyticsFacade _analyticsFacade;

    public DifferenceAnalyticsCommand(IAnalyticsFacade analyticsFacade)
    {
        _analyticsFacade = analyticsFacade;
    }

    public void Execute()
    {
        var difference = _analyticsFacade.GetDifferenceBetweenIncomeAndExpenses();
        Console.WriteLine($"Разница доходов и расходов: {difference}");
    }
}