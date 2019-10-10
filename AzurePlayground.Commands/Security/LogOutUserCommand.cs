using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class LogOutUserCommand : ILogOutUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        public LogOutUserCommand(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserLogOut> Execute(UserLogOut parameter) {
            var result = new CommandResult<UserLogOut>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user != null && user.IsActive) {
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        Type = UserEventType.LoggedOut
                    });

                    context.SaveChanges();
                }
                else {
                    // Since there is no user input, the user is not responsible for errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to log out {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }                
            }

            return result;
        }
    }
}
