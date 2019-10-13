using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class ActivateUserCommandHandler : IActivateUserCommandHandler {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ActivateUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserActivation> Execute(UserActivation parameter) {
            var result = new CommandResult<UserActivation>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // Any error that occurs gets the same message to prevent leaking information
                if (user == null
                    || user.Status != UserStatus.New
                    || !int.TryParse(parameter.ActivationCode, out int activationCode) || user.ActivationCode != activationCode) {

                    result.AddError(p => p.ActivationCode, "This activation code is invalid");

                    if (user != null) {
                        user.UserEvents.Add(new UserEvent() {
                            Date = DateTime.UtcNow,
                            Type = UserEventType.FailedActivation
                        });
                    }
                }
                else {
                    user.Status = UserStatus.Active;
                    user.ActivationCode = null;
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        Type = UserEventType.Activated
                    });
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}
