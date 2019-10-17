using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ChangeUserPasswordCommandHandler(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<ChangeUserPasswordCommand> Execute(ChangeUserPasswordCommand parameter) {
            var result = new CommandResult<ChangeUserPasswordCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user == null || user.Status != UserStatus.Active) {
                    // Since there is no user input for email, the user is not responsible for these errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to change password for {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }

                if (!parameter.NewPassword.Equals(parameter.ConfirmNewPassword, StringComparison.Ordinal)) {
                    result.AddError(p => p.ConfirmNewPassword, "New password and confirm new password must match");
                }

                if (!user.Password.Verify(parameter.CurrentPassword)) {
                    result.AddError(p => p.CurrentPassword, "Invalid password");
                }

                if (result.Success) {
                    user.Password = new Password(parameter.NewPassword);
                    user.AddEvent(UserEventType.PasswordChanged);
                }
                else {
                    user.AddEvent(UserEventType.FailedPasswordChange);
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}