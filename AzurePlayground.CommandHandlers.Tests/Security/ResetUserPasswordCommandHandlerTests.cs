using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public class ResetUserPasswordCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;

        public ResetUserPasswordCommandHandlerTests() {
            _repository = new UserRepository(_context);
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Succeeds() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Active);
            user.Password.Returns(new Password("test"));
            user.PasswordResetToken.Returns(new TemporaryPassword("test"));

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.Received().ResetPassword("test2");
            user.DidNotReceive().ResetPasswordFailed();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_For_Expired_Token() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Active);
            user.Password.Returns(new Password("test"));
            var token = new TemporaryPassword("test");
            typeof(TemporaryPassword).GetProperty(nameof(TemporaryPassword.ExpiryDate)).SetValue(token, DateTime.UtcNow.AddSeconds(-60));
            user.PasswordResetToken.Returns(token);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.DidNotReceive().ResetPassword(Arg.Any<string>());
            user.Received().ResetPasswordFailed();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_When_Already_Reset() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Active);
            user.Password.Returns(new Password("test"));
            user.PasswordResetToken.Returns(TemporaryPassword.None);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.DidNotReceive().ResetPassword(Arg.Any<string>());
            user.Received().ResetPasswordFailed();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_For_Incorrect_Token() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "wrong", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Active);
            user.Password.Returns(new Password("test"));
            user.PasswordResetToken.Returns(new TemporaryPassword("test"));

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.DidNotReceive().ResetPassword(Arg.Any<string>());
            user.Received().ResetPasswordFailed();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Inactive);
            user.Password.Returns(new Password("test"));
            user.PasswordResetToken.Returns(new TemporaryPassword("test"));

            _context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for inactive user 'test@test.com'");
            user.DidNotReceive().ResetPassword(Arg.Any<string>());
            user.DidNotReceive().ResetPasswordFailed();

        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Throws_Exception_For_New_User() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.New);
            user.Password.Returns(new Password("test"));
            user.PasswordResetToken.Returns(new TemporaryPassword("test"));

            _context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for inactive user 'test@test.com'");
            user.DidNotReceive().ResetPassword(Arg.Any<string>());
            user.DidNotReceive().ResetPasswordFailed();

        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new ResetUserPasswordCommandHandler(_repository);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2");

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for non-existent user 'test@test.com'");
        }
    }
}