using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public class ActivateUserCommandHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private UserRepository _repository;

        [TestInitialize]
        public void Initialize() {
            _repository = new UserRepository(_context);
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Succeeds() {
            var handler = new ActivateUserCommandHandler(_repository);
            var command = new ActivateUserCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.ActivationCode.Returns(999999);
            user.Status.Returns(UserStatus.New);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().BeEmpty();
            user.Received().Activate();
            user.DidNotReceive().ActivationFailed();
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Nonexistent_User() {
            var handler = new ActivateUserCommandHandler(_repository);
            var command = new ActivateUserCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("other@test.com");
            user.ActivationCode.Returns(999999);
            user.Status.Returns(UserStatus.New);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.DidNotReceive().Activate();
            user.DidNotReceive().ActivationFailed();
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Active_User() {
            var handler = new ActivateUserCommandHandler(_repository);
            var command = new ActivateUserCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.ActivationCode.Returns(999999);
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.DidNotReceive().Activate();
            user.Received().ActivationFailed();
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Inactive_User() {
            var handler = new ActivateUserCommandHandler(_repository);
            var command = new ActivateUserCommand("test@test.com", "999999");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.ActivationCode.Returns(999999);
            user.Status.Returns(UserStatus.Inactive);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.DidNotReceive().Activate();
            user.Received().ActivationFailed();
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Wrong_Code() {
            var handler = new ActivateUserCommandHandler(_repository);
            var command = new ActivateUserCommand("test@test.com", "999987");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.ActivationCode.Returns(999999);
            user.Status.Returns(UserStatus.New);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.DidNotReceive().Activate();
            user.Received().ActivationFailed();
        }
    }
}