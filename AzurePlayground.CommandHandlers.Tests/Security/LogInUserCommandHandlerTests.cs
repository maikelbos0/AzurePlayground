using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public class LogInUserCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

        [TestMethod]
        public void LogInUserCommandHandler_Succeeds() {
            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
            var command = new LogInUserCommand("test@test.com", "test");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Success.Should().BeTrue();
            user.Received().LogIn();
            user.DidNotReceive().LogInFailed();
        }

        [TestMethod]
        public void LogInUserCommandHandler_Fails_For_Nonexistent_User() {
            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
            var command = new LogInUserCommand("other@test.com", "test");

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("Invalid email or password");
        }

        [TestMethod]
        public void LogInUserCommandHandler_Fails_For_Inactive_User() {
            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
            var command = new LogInUserCommand("test@test.com", "test");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Inactive);

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.DidNotReceive().LogIn();
            user.Received().LogInFailed();
        }

        [TestMethod]
        public void LogInUserCommandHandler_Fails_For_New_User() {
            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
            var command = new LogInUserCommand("test@test.com", "test");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.New);

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.DidNotReceive().LogIn();
            user.Received().LogInFailed();
        }

        [TestMethod]
        public void LogInUserCommandHandler_Fails_For_Invalid_Password() {
            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
            var command = new LogInUserCommand("test@test.com", "wrong");
            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Password.Returns(new Password("test"));
            user.Status.Returns(UserStatus.Active);

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.Should().BeNull();
            result.Errors[0].Message.Should().Be("Invalid email or password");
            user.DidNotReceive().LogIn();
            user.Received().LogInFailed();
        }
    }
}