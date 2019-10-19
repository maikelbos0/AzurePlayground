using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class SendUserActivationCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void SendUserActivationCommandHandler_Sends_Email() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new SendUserActivationCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(new User("test@test.com", "test"));

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Succeeds() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new SendUserActivationCommand("test@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.ActivationCode.Returns(999999);
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.New);
            
            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.Received().GenerateActivationCode();
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Does_Nothing_For_Active_User() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new SendUserActivationCommand("other@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.DidNotReceive().GenerateActivationCode();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Does_Nothing_For_Inactive_User() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new SendUserActivationCommand("other@test.com");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Inactive);

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.DidNotReceive().GenerateActivationCode();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Does_Nothing_For_Nonexistent_User() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new SendUserActivationCommand("other@test.com");

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
        }
    }
}