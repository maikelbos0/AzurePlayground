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
    public class LogInUserCommandTests {
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
        public void LogInUserCommand_Succeeds() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
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
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.LoggedIn);
        }

        [TestMethod]
        public void LogInUserCommand_Resets_Password_Reset_Data() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true,
                PasswordResetTokenHash = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                PasswordResetTokenExpiryDate = new DateTime()
            });

            command.Execute(model);

            var user = _playgroundContextFactory.Context.Users.Single();

            user.PasswordResetTokenSalt.Should().BeNull();
            user.PasswordResetTokenHash.Should().BeNull();
            user.PasswordResetTokenHashIterations.Should().BeNull();
            user.PasswordResetTokenExpiryDate.Should().BeNull();
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Nonexistent_User() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "other@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Inactive_User() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedLogIn);
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Invalid_Password() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "wrong"
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
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedLogIn);
        }
    }
}
