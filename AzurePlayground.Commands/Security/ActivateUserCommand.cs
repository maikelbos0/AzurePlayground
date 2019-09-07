using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class ActivateUserCommand : IActivateUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ActivateUserCommand(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserActivation> Execute(UserActivation parameter) {
            var result = new CommandResult<UserActivation>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));
                int activationCode;

                // Any error that occurs gets the same message to prevent leaking information
                if (user == null
                    || user.IsActive
                    || !int.TryParse(parameter.ActivationCode, out activationCode) || user.ActivationCode != activationCode) {

                    result.AddError(p => p.ActivationCode, "This activation code is invalid");

                    if (user != null) {
                        user.UserEvents.Add(new UserEvent() {
                            Date = DateTime.UtcNow,
                            UserEventType = UserEventType.FailedActivation
                        });
                    }
                }
                else {
                    user.IsActive = true;
                    user.ActivationCode = null;
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType = UserEventType.Activated
                    });
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}
