using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class LogInUserCommandHandler : ICommandHandler<LogInUserCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        
        public LogInUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<LogInUserCommand> Execute(LogInUserCommand parameter) {
            var result = new CommandResult<LogInUserCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user == null || user.Status != UserStatus.Active || !user.Password.Verify(parameter.Password)) {
                    result.AddError("Invalid email or password");
                    user?.LogInFailed();
                }
                else {
                    user.LogIn();
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}