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
            var user = _repository.TryGetByEmail(parameter.Email);

            if (user == null || user.Status != UserStatus.Active) {
                // Since there is no user input for email, the user is not responsible for these errors and we should not use the command result for feedback
                throw new InvalidOperationException($"Attempted to change password for {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
            }

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