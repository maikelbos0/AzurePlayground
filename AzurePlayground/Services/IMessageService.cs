using AzurePlayground.CommandHandlers;
using AzurePlayground.Commands;
using AzurePlayground.Queries;

namespace AzurePlayground.Services {
    public interface IMessageService {
        CommandResult<TCommand> Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
        TReturnValue Dispatch<TReturnValue>(IQuery<TReturnValue> query);
    }
}