using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ActivateUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<ActivateUserCommand> Execute(ActivateUserCommand parameter) {
            var result = new CommandResult<ActivateUserCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // Any error that occurs gets the same message to prevent leaking information
                if (user == null || !int.TryParse(parameter.ActivationCode, out int activationCode) || !user.Activate(activationCode)) {
                    result.AddError(p => p.ActivationCode, "This activation code is invalid");
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}
