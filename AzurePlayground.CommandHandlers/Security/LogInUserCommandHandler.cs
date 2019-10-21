using AzurePlayground.CommandHandlers.Decorators;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    [Audit]
    public sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand> {
        private readonly IUserRepository _repository;

        public LogInUserCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<LogInUserCommand> Execute(LogInUserCommand parameter) {
            var result = new CommandResult<LogInUserCommand>();
            var user = _repository.TryGetByEmail(parameter.Email);

            if (user == null || user.Status != UserStatus.Active || !user.Password.Verify(parameter.Password)) {
                result.AddError("Invalid email or password");
                user?.LogInFailed();
            }
            else {
                user.LogIn();
            }

            _repository.Update();

            return result;
        }
    }
}