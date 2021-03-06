﻿using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using AzurePlayground.Repositories.Security;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public sealed class RegisterUserCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        public RegisterUserCommandHandlerTests() {
            _repository = new UserRepository(_context);
        }

        [TestMethod]
        public void RegisterUserCommandHandler_Succeeds() {
            var handler = new RegisterUserCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new RegisterUserCommand("test@test.com", "test");

            var result = handler.Execute(command);
            var user = _context.Users.SingleOrDefault();

            result.Errors.Should().BeEmpty();
            user.Should().NotBeNull();
            user.Status.Should().Be(UserStatus.New);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.Registered);
        }

        [TestMethod]
        public void RegisterUserCommandHandler_Sends_Email() {
            var handler = new RegisterUserCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new RegisterUserCommand("test@test.com", "test");

            handler.Execute(command);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void RegisterUserCommandHandler_Fails_For_Existing_Email() {
            var handler = new RegisterUserCommandHandler(_repository, _mailClient, new ActivationMailTemplate(_appSettings));
            var command = new RegisterUserCommand("test@test.com", "test");

            _context.Users.Add(new User("test@test.com", "test"));

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Email address already exists");
            _mailClient.SentMessages.Should().BeEmpty();
        }
    }
}