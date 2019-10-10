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
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true
            };
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.LoggedIn);
        }

        [TestMethod]
        public void LogInUserCommand_Resets_Password_Reset_Data() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true,
                PasswordResetToken = new TemporaryPassword("test")
            };
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            command.Execute(model);

            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Nonexistent_User() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "other@test.com",
                Password = "test"
            };

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Inactive_User() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test")
            };
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedLogIn);
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Invalid_Password() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                IsActive = true
            };
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedLogIn);
        }
    }
}