using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    public sealed class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand> {
        private readonly IUserRepository _repository;

        public ChangeUserPasswordCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<ChangeUserPasswordCommand> Execute(ChangeUserPasswordCommand parameter) {
            var result = new CommandResult<ChangeUserPasswordCommand>();
            var user = _repository.GetByEmail(parameter.Email, UserStatus.Active);

            if (user.Password.Verify(parameter.CurrentPassword)) {
                user.ChangePassword(parameter.NewPassword);
            }
            else { 
                result.AddError(p => p.CurrentPassword, "Invalid password");
                user.ChangePasswordFailed();
            }

            _repository.Update();

            return result;
        }
    }
}