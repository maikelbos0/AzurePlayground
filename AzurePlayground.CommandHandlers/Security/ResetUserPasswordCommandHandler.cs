﻿using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ResetUserPasswordCommandHandler : BaseUserCommandHandler, ICommandHandler<ResetUserPasswordCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ResetUserPasswordCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<ResetUserPasswordCommand> Execute(ResetUserPasswordCommand parameter) {
            var result = new CommandResult<ResetUserPasswordCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user == null || user.Status != UserStatus.Active) {
                    // Since there is no user input for email, the user is not responsible for these errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to reset password for {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }

                if (!user.PasswordResetToken.Verify(parameter.PasswordResetToken)) {
                    result.AddError(p => p.PasswordResetToken, "The password reset link has expired; please request a new one");
                }

                if (!parameter.NewPassword.Equals(parameter.ConfirmNewPassword, StringComparison.Ordinal)) {
                    result.AddError(p => p.ConfirmNewPassword, "New password and confirm new password must match");
                }

                if (result.Success) {
                    user.PasswordResetToken = TemporaryPassword.None;
                    user.Password = new Password(parameter.NewPassword);
                    user.AddEvent(UserEventType.PasswordReset);
                }
                else {
                    user.AddEvent(UserEventType.FailedPasswordReset);
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}