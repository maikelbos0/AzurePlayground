using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class ChangeUserPasswordCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Succeeds() {
            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.PasswordChanged);
            user.Password.Verify("test2").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            Action commandAction = () => {
                var result = handler.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for non-existent user 'test@test.com'");
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Inactive
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for inactive user 'test@test.com'");
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_New_User() {
            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.New
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for inactive user 'test@test.com'");
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Fails_For_Wrong_Password() {
            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "wrong",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.CurrentPassword");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordChange);
            user.Password.Verify("test").Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Fails_For_Unmatched_New_Password() {
            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmNewPassword");
            result.Errors[0].Message.Should().Be("New password and confirm new password must match");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordChange);
            user.Password.Verify("test").Should().BeTrue();
        }
    }
}