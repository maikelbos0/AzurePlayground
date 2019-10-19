using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand> {
        private readonly IPlaygroundContext _context;

        public DeactivateUserCommandHandler(IPlaygroundContext context) {
            _context = context;
        }

        public CommandResult<DeactivateUserCommand> Execute(DeactivateUserCommand parameter) {
            var result = new CommandResult<DeactivateUserCommand>();
            var user = _context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

            if (user != null && user.Status == UserStatus.Active) {
                if (user.Password.Verify(parameter.Password)) {
                    user.Deactivate();
                }
                else {
                    result.AddError(p => p.Password, "Invalid password");
                    user.DeactivationFailed();
                }

                _context.SaveChanges();
            }
            else {
                // Since there is no user input for email, the user is not responsible for errors and we should not use the command result for feedback
                throw new InvalidOperationException($"Attempted to deactivate {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
            }

            return result;
        }
    }
}