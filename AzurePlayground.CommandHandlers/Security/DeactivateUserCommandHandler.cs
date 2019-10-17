using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public DeactivateUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<DeactivateUserCommand> Execute(DeactivateUserCommand parameter) {
            var result = new CommandResult<DeactivateUserCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user != null && user.Status == UserStatus.Active) {
                    if (user.Password.Verify(parameter.Password)) {
                        user.Status = UserStatus.Inactive;
                        user.UserEvents.Add(new UserEvent() {
                            Date = DateTime.UtcNow,
                            Type = UserEventType.Deactivated
                        });
                    }
                    else {
                        result.AddError(p => p.Password, "Invalid password");
                        user.UserEvents.Add(new UserEvent() {
                            Date = DateTime.UtcNow,
                            Type = UserEventType.FailedDeactivation
                        });
                    }

                    context.SaveChanges();
                }
                else {
                    // Since there is no user input for email, the user is not responsible for errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to deactivate {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }
            }

            return result;
        }
    }
}