using AzurePlayground.Commands;
using AzurePlayground.Utilities.Container;

namespace AzurePlayground.CommandHandlers.Decorators {
    public sealed class AuditDecorator<TCommand> : Decorator<ICommandHandler<TCommand>>, ICommandHandler<TCommand> where TCommand : ICommand {
        public CommandResult<TCommand> Execute(TCommand command) {
            System.Console.WriteLine($"Called decorator for command {typeof(ICommand).Name} with handler {Handler.GetType().Name}");

            return Handler.Execute(command);
        }
    }
}