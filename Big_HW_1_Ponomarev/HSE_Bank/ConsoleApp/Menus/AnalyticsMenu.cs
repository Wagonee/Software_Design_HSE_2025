using HSE_Bank.Application.Commands;
using HSE_Bank.Application.Decorators;
using HSE_Bank.ConsoleApp.Utils;
using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.ConsoleApp.Menus;

public static class AnalyticsMenu
{
    private static IAnalyticsFacade _analyticsFacade;

    public static void Init(IAnalyticsFacade analyticsFacade)
    {
        _analyticsFacade = analyticsFacade;
    }

    public static void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("====== Аналитика ======");
            Console.WriteLine("1. Подсчёт разницы доходов и расходов (в общем)");
            Console.WriteLine("2. Группировка операций по типу (доходы/расходы)");
            Console.WriteLine("3. Назад");

            int input = ConsoleHelper.GetIntInput("Введите число для выбора опции: ", 1, 3);

            switch (input)
            {
                case 1:
                    ExecuteAnalyticsCommand(new DifferenceAnalyticsCommand(_analyticsFacade));
                    break;
                case 2:
                    ExecuteAnalyticsCommand(new GroupByTypeAnalyticsCommand(_analyticsFacade));
                    break;
                case 3:
                    return;
                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                    break;
            }
        }
    }

    private static void ExecuteAnalyticsCommand(ICommands command)
    {
        try
        {
            Console.Clear();
            var decoratedCommand = new ExecutionTimeDecorator(command);
            decoratedCommand.Execute();
            ConsoleHelper.WaitForKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ConsoleHelper.WaitForKey();
        }
    }
        
}