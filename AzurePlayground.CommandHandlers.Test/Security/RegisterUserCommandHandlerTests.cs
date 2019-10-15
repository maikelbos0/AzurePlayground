﻿using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class RegisterUserCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void RegisterUserCommandHandler_Succeeds() {
            var handler = new RegisterUserCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var command = new RegisterUserCommand("test@test.com", "test", "test");

            var result = handler.Execute(command);
            var user = _playgroundContextFactory.Context.Users.SingleOrDefault();

            result.Errors.Should().BeEmpty();
            user.Should().NotBeNull();
            user.Status.Should().Be(UserStatus.New);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.Registered);
        }

        [TestMethod]
        public void RegisterUserCommandHandler_Fails_For_Unmatched_Password() {
            var handler = new RegisterUserCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var command = new RegisterUserCommand("test@test.com", "test", "wrong");

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ConfirmPassword");
            result.Errors[0].Message.Should().Be("Password and confirm password must match");
        }

        [TestMethod]
        public void RegisterUserCommandHandler_Fails_For_Existing_Email() {
            var handler = new RegisterUserCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var command = new RegisterUserCommand("test@test.com", "test", "test");

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com"
            });

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Email address already exists");
        }


        [TestMethod]
        public void RegisterUserCommandHandler_Sends_Email() {
            var handler = new RegisterUserCommandHandler(_playgroundContextFactory, _mailClient, _appSettings);
            var command = new RegisterUserCommand("test@test.com", "test", "test");

            handler.Execute(command);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }
    }
}