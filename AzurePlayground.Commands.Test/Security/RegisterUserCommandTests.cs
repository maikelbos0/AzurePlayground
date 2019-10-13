using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class RegisterUserCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void RegisterUserCommand_Succeeds() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test",
                ConfirmPassword = "test"
            };

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.SingleOrDefault();

            result.Errors.Should().BeEmpty();
            user.Should().NotBeNull();
            user.Status.Should().Be(UserStatus.New);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.Registered);
        }

        [TestMethod]
        public void RegisterUserCommand_Fails_For_Unmatched_Password() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test",
                ConfirmPassword = "wrong"
            };

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmPassword");
            result.Errors[0].Message.Should().Be("Password and confirm password must match");
        }

        [TestMethod]
        public void RegisterUserCommand_Fails_For_Existing_Email() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test",
                ConfirmPassword = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com"
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Email address already exists");
        }


        [TestMethod]
        public void RegisterUserCommand_Sends_Email() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test",
                ConfirmPassword = "test"
            };

            command.Execute(model);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }
    }
}