using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AzurePlayground.Utilities.Mail;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class ForgotUserPasswordCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Sends_Email() {
            var handler = new ForgotUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            });

            handler.Execute(command);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Your password reset request");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Succeeds() {
            var handler = new ForgotUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var command = new ForgotUserPasswordCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.PasswordResetToken.Should().NotBe(TemporaryPassword.None);
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Does_Nothing_For_Nonexistent_User() {
            var handler = new ForgotUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Does_Nothing_For_Inactive_User() {
            var handler = new ForgotUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Inactive
            };
            var command = new ForgotUserPasswordCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Does_Nothing_For_New_User() {
            var handler = new ForgotUserPasswordCommandHandler(_playgroundContextFactory, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.New
            };
            var command = new ForgotUserPasswordCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
        }
    }
}