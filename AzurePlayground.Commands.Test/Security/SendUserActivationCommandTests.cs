﻿using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class SendUserActivationCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void SendUserActivationCommand_Sends_Email() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserSendActivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void SendUserActivationCommand_Creates_New_Activation_Code() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            };
            var model = new UserSendActivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().NotBe(999999);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().UserEventType.Should().Be(UserEventType.ActivationCodeSent);
        }

        [TestMethod]
        public void SendUserActivationCommand_Does_Nothing_For_Active_User() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = null,
                IsActive = true
            };
            var model = new UserSendActivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().BeNull();
            user.UserEvents.Should().BeEmpty();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void SendUserActivationCommand_Does_Nothing_For_Nonexistent_User() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            };
            var model = new UserSendActivation() {
                Email = "other@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            user.ActivationCode.Should().Be(999999);
            _mailClient.SentMessages.Should().BeEmpty();
        }
    }
}
