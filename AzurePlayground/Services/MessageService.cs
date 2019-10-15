using AzurePlayground.CommandHandlers;
using AzurePlayground.Commands;
using AzurePlayground.Queries;
using Unity;

namespace AzurePlayground.Services {
    public class MessageService : IMessageService {
        private readonly IUnityContainer _container;

        public MessageService(IUnityContainer container) {
            _container = container;
        }

        public CommandResult<TCommand> Dispatch<TCommand>(TCommand command) where TCommand : ICommand {
            throw new System.NotImplementedException();
        }

        public TReturnValue Dispatch<TQuery, TReturnValue>(TQuery query) where TQuery : IQuery<TReturnValue> {
            throw new System.NotImplementedException();
        }
    }
}