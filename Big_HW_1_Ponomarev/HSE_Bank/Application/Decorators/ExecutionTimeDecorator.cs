using System.Windows.Input;
using HSE_Bank.Domain.Interfaces.Commands;
using Microsoft.Extensions.Logging;
using HSE_Bank.Domain.Interfaces.Decorators;

namespace HSE_Bank.Application.Decorators;

public class ExecutionTimeDecorator : ICommandDecorator
{
    public ICommands Command { get; }
    private ILogger<ExecutionTimeDecorator> _logger;

    public ExecutionTimeDecorator(ICommands command, ILogger<ExecutionTimeDecorator> logger)
    {
        Command = command;
        _logger = logger;
    }

    public void Execute()
    {
        var executionTime = DateTime.Now;
        Command.Execute();
        var endTime = DateTime.Now;
        var elapsedTime = endTime - executionTime;
        _logger.LogInformation($"Command{Command.GetType().Name} is executed in {elapsedTime.TotalMilliseconds} milliseconds.");
    }
}