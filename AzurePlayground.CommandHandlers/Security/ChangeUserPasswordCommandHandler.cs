using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand> {
        private readonly IPlaygroundContext _context;

        public ChangeUserPasswordCommandHandler(IPlaygroundContext context) {
            _context = context;
        }

        public CommandResult<ChangeUserPasswordCommand> Execute(ChangeUserPasswordCommand parameter) {
            var result = new CommandResult<ChangeUserPasswordCommand>();
            var user = _context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

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

            _context.SaveChanges();

            return result;
        }
    }
}