using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class ResetUserPasswordCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ResetUserPasswordCommand_Succeeds() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.PasswordReset.Id);
            user.Password.Verify("test2").Should().BeTrue();
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_For_Unmatched_New_Password() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmNewPassword");
            result.Errors[0].Message.Should().Be("New password and confirm new password must match");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_For_Expired_Token() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true,
                PasswordResetToken = new TemporaryPassword("test")
            };
            typeof(TemporaryPassword).GetProperty(nameof(TemporaryPassword.ExpiryDate)).SetValue(user.PasswordResetToken, DateTime.UtcNow.AddSeconds(-60));
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.PasswordResetToken");
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_When_Already_Reset() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true
            };
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.PasswordResetToken");
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_For_Incorrect_Token() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = "wrong",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.PasswordResetToken");
            result.Errors[0].Message.Should().Be("The password reset link has expired; please request a new one");
            user.Password.Verify("test").Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Throws_Exception_For_Inactive_User() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = false
            };
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = null,
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for inactive user 'test@test.com'");
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Throws_Exception_For_Nonexistent_User() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = null,
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password for non-existent user 'test@test.com'");
        }
    }
}
