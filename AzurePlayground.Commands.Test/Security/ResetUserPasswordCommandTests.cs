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
    public class ResetUserPasswordCommandTests {
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
        public void ResetUserPasswordCommand_Succeeds() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true,
                PasswordResetToken = new Password("test", DateTime.UtcNow.AddSeconds(3600))
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

            using (var pbkdf2 = new Rfc2898DeriveBytes("test2", user.PasswordSalt, user.PasswordHashIterations)) {
                user.PasswordHash.Should().BeEquivalentTo(pbkdf2.GetBytes(20), options => options.WithStrictOrdering());
            }
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_For_Unmatched_New_Password() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true,
                PasswordResetToken = new Password("test", DateTime.UtcNow.AddSeconds(3600))
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
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash, options => options.WithStrictOrdering());
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt, options => options.WithStrictOrdering());
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_For_Expired_Token() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true,
                PasswordResetToken = new Password("test", DateTime.UtcNow.AddSeconds(-60))
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
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash, options => options.WithStrictOrdering());
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt, options => options.WithStrictOrdering());
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Fails_When_Already_Reset() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
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
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash, options => options.WithStrictOrdering());
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt, options => options.WithStrictOrdering());
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType_Id.Should().Be(UserEventType.FailedPasswordReset.Id);
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Throws_Exception_For_Inactive_User() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
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
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash, options => options.WithStrictOrdering());
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt, options => options.WithStrictOrdering());
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

        [TestMethod]
        public void ResetUserPasswordCommand_Throws_Exception_For_Missing_Token() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true,
                PasswordResetToken = new Password("test", DateTime.UtcNow.AddSeconds(3600))
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

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password with missing token for user 'test@test.com'");
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash, options => options.WithStrictOrdering());
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt, options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void ResetUserPasswordCommand_Throws_Exception_For_Incorrect_Token() {
            var command = new ResetUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true,
                PasswordResetToken = new Password("test", DateTime.UtcNow.AddSeconds(3600))
            };
            var model = new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = "wrong",
                NewPassword = "test2",
                ConfirmNewPassword = "test2"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to reset password with incorrect token for user 'test@test.com'");
            user.PasswordHash.Should().BeEquivalentTo(_passwordHash, options => options.WithStrictOrdering());
            user.PasswordSalt.Should().BeEquivalentTo(_passwordSalt, options => options.WithStrictOrdering());
        }
    }
}
