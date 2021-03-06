﻿using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public sealed class ConfirmUserEmailChangeCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;

        public ConfirmUserEmailChangeCommandHandlerTests() {
            _repository = new UserRepository(_context);
        }

        [TestMethod]
        public void ConfirmUserEmailChangeCommandHandler_Succeeds() {
            var handler = new ConfirmUserEmailChangeCommandHandler(_repository);
            var command = new ConfirmUserEmailChangeCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.NewEmailConfirmationCode.Returns(999999);
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().BeEmpty();
            user.Received().ChangeEmail();
            user.DidNotReceive().ChangeEmailFailed();
        }

        [TestMethod]
        public void ConfirmUserEmailChangeCommandHandler_Fails_For_Nonexistent_User() {
            var handler = new ConfirmUserEmailChangeCommandHandler(_repository);
            var command = new ConfirmUserEmailChangeCommand("test@test.com", "999999");

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("c => c.ConfirmationCode");
            result.Errors[0].Message.Should().Be("This confirmation code is invalid");
        }

        [TestMethod]
        public void ConfirmUserEmailChangeCommandHandler_Fails_For_New_User() {
            var handler = new ConfirmUserEmailChangeCommandHandler(_repository);
            var command = new ConfirmUserEmailChangeCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.NewEmailConfirmationCode.Returns(999999);
            user.Status.Returns(UserStatus.New);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("c => c.ConfirmationCode");
            result.Errors[0].Message.Should().Be("This confirmation code is invalid");
            user.DidNotReceive().ChangeEmail();
            user.Received().ChangeEmailFailed();
        }

        [TestMethod]
        public void ConfirmUserEmailChangeCommandHandler_Fails_For_Inactive_User() {
            var handler = new ConfirmUserEmailChangeCommandHandler(_repository);
            var command = new ConfirmUserEmailChangeCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.NewEmailConfirmationCode.Returns(999999);
            user.Status.Returns(UserStatus.Inactive);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("c => c.ConfirmationCode");
            result.Errors[0].Message.Should().Be("This confirmation code is invalid");
            user.DidNotReceive().ChangeEmail();
            user.Received().ChangeEmailFailed();
        }

        [TestMethod]
        public void ConfirmUserEmailChangeCommandHandler_Fails_For_Wrong_Code() {
            var handler = new ConfirmUserEmailChangeCommandHandler(_repository);
            var command = new ConfirmUserEmailChangeCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.NewEmailConfirmationCode.Returns(555555);
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("c => c.ConfirmationCode");
            result.Errors[0].Message.Should().Be("This confirmation code is invalid");
            user.DidNotReceive().ChangeEmail();
            user.Received().ChangeEmailFailed();
        }
    }
}