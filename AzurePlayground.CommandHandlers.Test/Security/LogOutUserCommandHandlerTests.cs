using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class LogOutUserCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

        [TestMethod]
        public void LogOutUserCommandHandler_Succeeds() {
            var handler = new LogOutUserCommandHandler(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                Status = UserStatus.Active
            };
            var model = new UserLogOut() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.LoggedOut);
        }

        [TestMethod]
        public void LogOutUserCommandHandler_Throws_Exception_For_New_User() {
            var handler = new LogOutUserCommandHandler(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = handler.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Status = UserStatus.New
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out inactive user 'test@test.com'");
        }

        [TestMethod]
        public void LogOutUserCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new LogOutUserCommandHandler(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = handler.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Status = UserStatus.Inactive
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out inactive user 'test@test.com'");
        }

        [TestMethod]
        public void LogOutUserCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new LogOutUserCommandHandler(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = handler.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out non-existent user 'test@test.com'");
        }
    }
}