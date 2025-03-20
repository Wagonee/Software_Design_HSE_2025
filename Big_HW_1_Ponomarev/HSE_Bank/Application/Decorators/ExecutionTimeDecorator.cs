using System.Windows.Input;
using HSE_Bank.Domain.Interfaces.Commands;
using Microsoft.Extensions.Logging;
using HSE_Bank.Domain.Interfaces.Decorators;

namespace HSE_Bank.Application.Decorators;

public class ExecutionTimeDecorator : ICommandDecorator
{
    public ICommands Command { get; }

    public ExecutionTimeDecorator(ICommands command)
    {
        Command = command;
    }

    public void Execute()
    {
        var executionTime = DateTime.Now;
        Command.Execute();
        var endTime = DateTime.Now;
        var elapsedTime = endTime - executionTime;
        Console.WriteLine($"Elapsed time of {Command.GetType().Name}: {elapsedTime}");
    }
}