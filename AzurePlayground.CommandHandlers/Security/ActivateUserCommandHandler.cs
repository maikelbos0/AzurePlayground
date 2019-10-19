using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand> {
        private readonly IPlaygroundContext _context;

        public ActivateUserCommandHandler(IPlaygroundContext context) {
            _context = context;
        }

        public CommandResult<ActivateUserCommand> Execute(ActivateUserCommand parameter) {
            var result = new CommandResult<ActivateUserCommand>();
            var user = _context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

            // Any error that occurs gets the same message to prevent leaking information
            if (user == null
                || !int.TryParse(parameter.ActivationCode, out int activationCode)
                || user.Status != UserStatus.New
                || user.ActivationCode != activationCode) {

                user?.ActivationFailed();
                result.AddError(p => p.ActivationCode, "This activation code is invalid");
            }
            else {
                user.Activate();
            }

            _context.SaveChanges();

            return result;
        }
    }
}
