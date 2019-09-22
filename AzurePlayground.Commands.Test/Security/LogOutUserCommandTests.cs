using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class LogOutUserCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

        [TestMethod]
        public void LogOutUserCommand_Succeeds() {
            var command = new LogOutUserCommand(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = true
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.LoggedOut);
        }

        [TestMethod]
        public void LogOutUserCommand_Throws_Exception_For_Inactive_User() {
            var command = new LogOutUserCommand(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = command.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = false
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out inactive user 'test@test.com'");
        }

        [TestMethod]
        public void LogOutUserCommand_Throws_Exception_For_Nonexistent_User() {
            var command = new LogOutUserCommand(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out non-existent user 'test@test.com'");
        }
    }
}
