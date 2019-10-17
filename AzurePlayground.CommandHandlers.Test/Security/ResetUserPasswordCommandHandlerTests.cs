using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class ResetUserPasswordCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Succeeds() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "test2");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.PasswordReset);
            user.Password.Verify("test2").Should().BeTrue();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_For_Unmatched_New_Password() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "wrong");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmNewPassword");
            result.Errors[0].Message.Should().Be("New password and confirm new password must match");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordReset);
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_For_Expired_Token() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active,
                PasswordResetToken = new TemporaryPassword("test")
            };
            typeof(TemporaryPassword).GetProperty(nameof(TemporaryPassword.ExpiryDate)).SetValue(user.PasswordResetToken, DateTime.UtcNow.AddSeconds(-60));
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "test2");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordReset);
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_When_Already_Reset() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "test2");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordReset);
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Fails_For_Incorrect_Token() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var command = new ResetUserPasswordCommand("test@test.com", "wrong", "test2", "test2");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordReset);
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Inactive
            };
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "test2");

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for inactive user 'test@test.com'");
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Throws_Exception_For_New_User() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.New
            };
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "test2");

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for inactive user 'test@test.com'");
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ResetUserPasswordCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new ResetUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var command = new ResetUserPasswordCommand("test@test.com", "test", "test2", "test2"); 

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for non-existent user 'test@test.com'");
        }
    }
}