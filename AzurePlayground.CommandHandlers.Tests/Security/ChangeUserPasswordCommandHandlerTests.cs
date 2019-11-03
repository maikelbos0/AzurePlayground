using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public sealed class ChangeUserPasswordCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;

        public ChangeUserPasswordCommandHandlerTests() {
            _repository = new UserRepository(_context);
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Succeeds() {
            var handler = new ChangeUserPasswordCommandHandler(_repository);
            var command = new ChangeUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.Received().ChangePassword("test2");
            user.DidNotReceive().ChangePasswordFailed();
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_Invalid_User() {
            var handler = new ChangeUserPasswordCommandHandler(_repository);
            var command = new ChangeUserPasswordCommand("test@test.com", "test", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Inactive);

            _context.Users.Add(user);

            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>();
            user.DidNotReceive().ChangePassword(Arg.Any<string>());
            user.DidNotReceive().ChangePasswordFailed();
        }

        [TestMethod]
        public void ChangeUserPasswordCommandHandler_Fails_For_Wrong_Password() {
            var handler = new ChangeUserPasswordCommandHandler(_repository);
            var command = new ChangeUserPasswordCommand("test@test.com", "wrong", "test2");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.CurrentPassword");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.DidNotReceive().ChangePassword(Arg.Any<string>());
            user.Received().ChangePasswordFailed();
        }
    }
}