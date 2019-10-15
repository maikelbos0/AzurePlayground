using AzurePlayground.Commands;

namespace AzurePlayground.CommandHandlers {
    public interface ICommandHandler<TCommand> where TCommand : ICommand {
        CommandResult<TCommand> Execute(TCommand command);
    }
}