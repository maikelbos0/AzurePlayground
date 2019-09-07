using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AzurePlayground.Commands.Test {
    [TestClass]
    public class SecurityTests {
        private FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private FakeMailClient _mailClient = new FakeMailClient();
        private FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };
        
        [TestMethod]
        public void RegisterUserCommand_Succeeds() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test"
            };

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();
            _playgroundContextFactory.Context.Users.Should().HaveCount(1);
        }


        [TestMethod]
        public void RegisterUserCommand_Returns_Error_For_Existing_Email() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test"
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
                Password = "test"
            };

            command.Execute(model);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }
    }
}