﻿using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    public sealed class ResetUserPasswordCommandHandler : ICommandHandler<ResetUserPasswordCommand> {
        private readonly IUserRepository _repository;

        public ResetUserPasswordCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<ResetUserPasswordCommand> Execute(ResetUserPasswordCommand parameter) {
            var result = new CommandResult<ResetUserPasswordCommand>();
            var user = _repository.GetActiveByEmail(parameter.Email);

            if (user.PasswordResetToken.Verify(parameter.PasswordResetToken)) {
                user.ResetPassword(parameter.NewPassword);
            }
            else {
                result.AddError("The password reset link has expired; please request a new one");
                user.ResetPasswordFailed();
            }

            _repository.Update();

            return result;
        }
    }
}