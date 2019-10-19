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
    public class DeactivateUserCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();

        [TestMethod]
        public void DeactivateUserCommandHandler_Succeeds() {
            var handler = new DeactivateUserCommandHandler(_context);
            var command = new DeactivateUserCommand("test@test.com", "test");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().BeEmpty();
            user.Received().Deactivate();
            user.DidNotReceive().DeactivationFailed();
        }

        [TestMethod]
        public void DeactivateUserCommandHandler_Fails_For_Wrong_Password() {
            var handler = new DeactivateUserCommandHandler(_context);
            var command = new DeactivateUserCommand("test@test.com", "wrong");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Password");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.DidNotReceive().Deactivate();
            user.Received().DeactivationFailed();
        }

        [TestMethod]
        public void DeactivateUserCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new DeactivateUserCommandHandler(_context);
            var command = new DeactivateUserCommand("test@test.com", "test");

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate non-existent user 'test@test.com'");
        }

        [TestMethod]
        public void DeactivateUserCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new DeactivateUserCommandHandler(_context);
            var command = new DeactivateUserCommand("test@test.com", "test");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Inactive);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate inactive user 'test@test.com'");
            user.DidNotReceive().Deactivate();
            user.DidNotReceive().DeactivationFailed();
        }

        [TestMethod]
        public void DeactivateUserCommandHandler_Throws_Exception_For_New_User() {
            var handler = new DeactivateUserCommandHandler(_context);
            var command = new DeactivateUserCommand("test@test.com", "test");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.New);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate inactive user 'test@test.com'");
            user.DidNotReceive().Deactivate();
            user.DidNotReceive().DeactivationFailed();
        }
    }
}