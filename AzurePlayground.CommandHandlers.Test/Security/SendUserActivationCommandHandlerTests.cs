using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var command = new SendUserActivationCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.New
            });

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Creates_New_Activation_Code() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.New
            };
            var command = new SendUserActivationCommand("test@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().NotBe(999999);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.ActivationCodeSent);
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Does_Nothing_For_Active_User() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = null,
                Status = UserStatus.Active
            };
            var command = new SendUserActivationCommand("other@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().BeNull();
            user.UserEvents.Should().BeEmpty();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Does_Nothing_For_Inactive_User() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = null,
                Status = UserStatus.Inactive
            };
            var command = new SendUserActivationCommand("other@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().BeNull();
            user.UserEvents.Should().BeEmpty();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void SendUserActivationCommandHandler_Does_Nothing_For_Nonexistent_User() {
            var handler = new SendUserActivationCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.New
            };
            var command = new SendUserActivationCommand("other@test.com");

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().Be(999999);
            _mailClient.SentMessages.Should().BeEmpty();
        }
    }
}