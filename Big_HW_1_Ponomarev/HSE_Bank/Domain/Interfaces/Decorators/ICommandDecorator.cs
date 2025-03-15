using System.Windows.Input;
using HSE_Bank.Domain.Interfaces.Commands;

namespace HSE_Bank.Domain.Interfaces.Decorators;

public interface ICommandDecorator : ICommands
{
    ICommands Command { get; }
}