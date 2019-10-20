﻿using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand> {
        private readonly IUserRepository _repository;

        public DeactivateUserCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<DeactivateUserCommand> Execute(DeactivateUserCommand parameter) {
            var result = new CommandResult<DeactivateUserCommand>();
            var user = _repository.TryGetByEmail(parameter.Email);

            if (user != null && user.Status == UserStatus.Active) {
                if (user.Password.Verify(parameter.Password)) {
                    user.Deactivate();
                }
                else {
                    result.AddError(p => p.Password, "Invalid password");
                    user.DeactivationFailed();
                }

                _repository.Update();
            }
            else {
                // Since there is no user input for email, the user is not responsible for errors and we should not use the command result for feedback
                throw new InvalidOperationException($"Attempted to deactivate {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
            }

            return result;
        }
    }
}