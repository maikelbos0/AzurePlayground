﻿using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class LogOutUserCommandHandler : ICommandHandler<LogOutUserCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        public LogOutUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<LogOutUserCommand> Execute(LogOutUserCommand parameter) {
            var result = new CommandResult<LogOutUserCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user != null && user.Status == UserStatus.Active) {
                    user.AddEvent(UserEventType.LoggedOut);

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
