using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public class LogOutUserCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();

        [TestMethod]
        public void LogOutUserCommandHandler_Succeeds() {
            var handler = new LogOutUserCommandHandler(_context);
            var command = new LogOutUserCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.Received().LogOut();
        }

        [TestMethod]
        public void LogOutUserCommandHandler_Throws_Exception_For_New_User() {
            var handler = new LogOutUserCommandHandler(_context);
            var command = new LogOutUserCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.New);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out inactive user 'test@test.com'");
            user.DidNotReceive().LogOut();
        }

        [TestMethod]
        public void LogOutUserCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new LogOutUserCommandHandler(_context);
            var command = new LogOutUserCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Inactive);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out inactive user 'test@test.com'");
            user.DidNotReceive().LogOut();

        }

        [TestMethod]
        public void LogOutUserCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new LogOutUserCommandHandler(_context);
            var command = new LogOutUserCommand("test@test.com");
            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out non-existent user 'test@test.com'");
        }
    }
}