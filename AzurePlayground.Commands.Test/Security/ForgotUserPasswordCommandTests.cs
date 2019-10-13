using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class ForgotUserPasswordCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ForgotUserPasswordCommand_Sends_Email() {
            var command = new ForgotUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserForgotPassword() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            });

            command.Execute(model);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Your password reset request");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void ForgotUserPasswordCommand_Succeeds() {
            var command = new ForgotUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var model = new UserForgotPassword() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.PasswordResetToken.Should().NotBe(TemporaryPassword.None);
        }

        [TestMethod]
        public void ForgotUserPasswordCommand_Does_Nothing_For_Nonexistent_User() {
            var command = new ForgotUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserForgotPassword() {
                Email = "test@test.com"
            };

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ForgotUserPasswordCommand_Does_Nothing_For_Inactive_User() {
            var command = new ForgotUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Inactive
            };
            var model = new UserForgotPassword() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
        }

        [TestMethod]
        public void ForgotUserPasswordCommand_Does_Nothing_For_New_User() {
            var command = new ForgotUserPasswordCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.New
            };
            var model = new UserForgotPassword() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
        }
    }
}