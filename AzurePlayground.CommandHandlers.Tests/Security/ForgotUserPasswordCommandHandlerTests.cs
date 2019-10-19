using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AzurePlayground.Utilities.Mail;
using NSubstitute;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public class ForgotUserPasswordCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Sends_Email() {
            var handler = new ForgotUserPasswordCommandHandler(_context, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            handler.Execute(command);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Your password reset request");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Succeeds() {
            var handler = new ForgotUserPasswordCommandHandler(_context, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.Received().GeneratePasswordResetToken();
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Does_Nothing_For_Nonexistent_User() {
            var handler = new ForgotUserPasswordCommandHandler(_context, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Does_Nothing_For_Inactive_User() {
            var handler = new ForgotUserPasswordCommandHandler(_context, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Inactive);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.DidNotReceive().GeneratePasswordResetToken();
        }

        [TestMethod]
        public void ForgotUserPasswordCommandHandler_Does_Nothing_For_New_User() {
            var handler = new ForgotUserPasswordCommandHandler(_context, _mailClient, new PasswordResetMailTemplate(_appSettings));
            var command = new ForgotUserPasswordCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.New);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.DidNotReceive().GeneratePasswordResetToken();
        }
    }
}