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
    public class ChangeUserPasswordCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ChangeUserPasswordCommand_Succeeds() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.PasswordChanged);
            user.Password.Verify("test2").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommand_Throws_Exception_For_Nonexistent_User() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for non-existent user 'test@test.com'");
        }

        [TestMethod]
        public void ChangeUserPasswordCommand_Throws_Exception_For_Inactive_User() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = false
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for inactive user 'test@test.com'");
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommand_Fails_For_Wrong_Password() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "wrong",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.CurrentPassword");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordChange);
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommand_Fails_For_Unmatched_New_Password() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmNewPassword");
            result.Errors[0].Message.Should().Be("New password and confirm new password must match");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordChange);
            user.Password.Verify("test").Should().BeTrue();
        }
    }
}