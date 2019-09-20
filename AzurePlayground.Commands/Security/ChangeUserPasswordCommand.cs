﻿using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class ChangeUserPasswordCommand : BaseUserCommand, IChangeUserPasswordCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ChangeUserPasswordCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserChangePassword> Execute(UserChangePassword parameter) {
            var result = new CommandResult<UserChangePassword>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user == null || !user.IsActive) {
                    // Since there is no user input for email, the user is not responsible for these errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to change password for {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }

                if (!parameter.NewPassword.Equals(parameter.ConfirmNewPassword, StringComparison.Ordinal)) {
                    result.AddError(p => p.ConfirmNewPassword, "New password and confirm new password must match");
                }

                if (!user.PasswordHash.SequenceEqual(GetPasswordHash(parameter.CurrentPassword, user.PasswordSalt, user.PasswordHashIterations))) {
                    result.AddError(p => p.CurrentPassword, "Invalid password");
                }

                if (result.Success) {
                    user.PasswordSalt = GetNewPasswordSalt();
                    user.PasswordHashIterations = _passwordHashIterations;
                    user.PasswordHash = GetPasswordHash(parameter.NewPassword, user.PasswordSalt, user.PasswordHashIterations);

                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType = UserEventType.PasswordChanged
                    });
                }
                else {
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType = UserEventType.FailedPasswordChange
                    });
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}