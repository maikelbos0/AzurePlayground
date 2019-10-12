using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class DeactivateUserCommand : IDeactivateUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public DeactivateUserCommand(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserDeactivation> Execute(UserDeactivation parameter) {
            var result = new CommandResult<UserDeactivation>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user != null && user.IsActive) {
                    user.IsActive = false;
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        Type = UserEventType.Deactivated
                    });

                    context.SaveChanges();
                }
                else {
                    // Since there is no user input, the user is not responsible for errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to deactivate {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }
            }

            return result;
        }
    }
}