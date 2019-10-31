using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public sealed class ChangeUserEmailCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        public ChangeUserEmailCommandHandlerTests() {
            _repository = new UserRepository(_context);
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Succeeds() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "test", "new@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.ActivationCode.Returns(999999);
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            handler.Execute(command);

            user.Received().ChangeEmail("new@test.com");
            user.DidNotReceive().ChangeEmailFailed();
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Sends_Email() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "test", "new@test.com");
            var user = new User("test@test.com", "test");

            user.Activate();
            _context.Users.Add(user);

            handler.Execute(command);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("new@test.com");
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Fails_For_Wrong_Password() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "wrong", "new@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("c => c.Password");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.DidNotReceive().ChangeEmail(Arg.Any<string>());
            user.Received().ChangeEmailFailed();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Fails_For_Existing_New_Email() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "test", "new@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var user2 = Substitute.For<User>();
            user2.Email.Returns("new@test.com");

            _context.Users.Add(user2);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("c => c.NewEmail");
            result.Errors[0].Message.Should().Be("Email address already exists");
            user.DidNotReceive().ChangeEmail(Arg.Any<string>());
            user.Received().ChangeEmailFailed();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "test", "new@test.com");

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change email for non-existent user 'test@test.com'");

            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Throws_Exception_For_New_User() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "test", "new@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.New);

            _context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change email for inactive user 'test@test.com'");

            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void ChangeUserEmailCommandHandler_Throws_Exception_For_Inactive_User() {
            var handler = new ChangeUserEmailCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new ChangeUserEmailCommand("test@test.com", "test", "new@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Inactive);

            _context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change email for inactive user 'test@test.com'");

            _mailClient.SentMessages.Should().BeEmpty();
        }
    }
}