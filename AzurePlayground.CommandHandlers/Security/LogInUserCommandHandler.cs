using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class LogInUserCommandHandler : ICommandHandler<LogInUserCommand> {
        private readonly IPlaygroundContext _context;

        public LogInUserCommandHandler(IPlaygroundContext playgroundContext) {
            _context = playgroundContext;
        }

        public CommandResult<LogInUserCommand> Execute(LogInUserCommand parameter) {
            var result = new CommandResult<LogInUserCommand>();
            var user = _context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

            if (user == null || user.Status != UserStatus.Active || !user.Password.Verify(parameter.Password)) {
                result.AddError("Invalid email or password");
                user?.LogInFailed();
            }
            else {
                user.LogIn();
            }

            _context.SaveChanges();

            return result;
        }
    }
}