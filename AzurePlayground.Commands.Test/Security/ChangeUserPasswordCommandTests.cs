using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class ChangeUserPasswordCommandTests {
        private readonly byte[] _passwordHash = new byte[] { 248, 212, 57, 28, 32, 158, 38, 248, 82, 175, 53, 217, 161, 238, 108, 226, 48, 123, 118, 173 };
        private readonly int _passwordHashIterations = 1000;
        private readonly byte[] _passwordSalt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
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
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.PasswordChanged);

            using (var pbkdf2 = new Rfc2898DeriveBytes("test2", user.PasswordSalt, user.PasswordHashIterations)) {
                user.PasswordHash.Should().BeEquivalentTo(pbkdf2.GetBytes(20), options => options.WithStrictOrdering());
            }
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
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = false
            });

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            var user = _playgroundContextFactory.Context.Users.Single();

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for inactive user 'test@test.com'");
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash);
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt);
        }

        [TestMethod]
        public void ChangeUserPasswordCommand_Fails_For_Wrong_Password() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "wrong",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.CurrentPassword");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedPasswordChange);
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash);
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt);
        }

        [TestMethod]
        public void ChangeUserPasswordCommand_Fails_For_Unmatched_New_Password() {
            var command = new ChangeUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserChangePassword() {
                Email = "test@test.com",
                CurrentPassword = "test",
                NewPassword = "test2",
                ConfirmNewPassword = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmNewPassword");
            result.Errors[0].Message.Should().Be("New password and confirm new password must match");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedPasswordChange);
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash);
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt);
        }
    }
}
