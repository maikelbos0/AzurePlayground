using AzurePlayground.CommandHandlers;
using AzurePlayground.Commands;
using AzurePlayground.Queries;
using AzurePlayground.QueryHandlers;
using AzurePlayground.Utilities.Container;
using Unity;

namespace AzurePlayground.Services {
    [InterfaceInjectable]
    public sealed class MessageService : IMessageService {
        private readonly IUnityContainer _container;

        public MessageService(IUnityContainer container) {
            _container = container;
        }

        public CommandResult<TCommand> Dispatch<TCommand>(TCommand command) where TCommand : ICommand {
            var handler = _container.Resolve<ICommandHandler<TCommand>>();

            return handler.Execute(command);
        }

        public TReturnValue Dispatch<TReturnValue>(IQuery<TReturnValue> query) {
            var type = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TReturnValue));
            dynamic handler = _container.Resolve(type);

            return handler.Execute((dynamic)query);
        }
    }
}