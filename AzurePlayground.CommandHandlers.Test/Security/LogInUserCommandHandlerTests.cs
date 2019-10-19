//using AzurePlayground.CommandHandlers.Security;
//using AzurePlayground.Commands.Security;
//using AzurePlayground.Domain.Security;
//using AzurePlayground.Test.Utilities;
//using FluentAssertions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Linq;

//namespace AzurePlayground.CommandHandlers.Test.Security {
//    [TestClass]
//    public class LogInUserCommandHandlerTests {
//        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

//        [TestMethod]
//        public void LogInUserCommandHandler_Succeeds() {
//            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Active
//            };
//            var command = new LogInUserCommand("test@test.com", "test");

//            _playgroundContextFactory.Context.Users.Add(user);

//            var result = handler.Execute(command);

//            result.Success.Should().BeTrue();
//            user.UserEvents.Should().HaveCount(1);
//            user.UserEvents.Single().Type.Should().Be(UserEventType.LoggedIn);
//        }

//        [TestMethod]
//        public void LogInUserCommandHandler_Resets_Password_Reset_Data() {
//            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Active,
//                PasswordResetToken = new TemporaryPassword("test")
//            };
//            var command = new LogInUserCommand("test@test.com", "test");

//            _playgroundContextFactory.Context.Users.Add(user);

//            handler.Execute(command);

//            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
//        }

//        [TestMethod]
//        public void LogInUserCommandHandler_Fails_For_Nonexistent_User() {
//            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
//            var command = new LogInUserCommand("other@test.com", "test");

//            var result = handler.Execute(command);

//            result.Errors.Should().HaveCount(1);
//            result.Errors[0].Expression.Should().BeNull();
//            result.Errors[0].Message.Should().Be("Invalid email or password");
//        }

//        [TestMethod]
//        public void LogInUserCommandHandler_Fails_For_Inactive_User() {
//            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Inactive
//            };
//            var command = new LogInUserCommand("test@test.com", "test");

//            _playgroundContextFactory.Context.Users.Add(user);

//            var result = handler.Execute(command);

//            result.Errors.Should().HaveCount(1);
//            result.Errors[0].Expression.Should().BeNull();
//            result.Errors[0].Message.Should().Be("Invalid email or password");
//            user.UserEvents.Should().HaveCount(1);
//            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedLogIn);
//        }

//        [TestMethod]
//        public void LogInUserCommandHandler_Fails_For_New_User() {
//            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.New
//            };
//            var command = new LogInUserCommand("test@test.com", "test");

//            _playgroundContextFactory.Context.Users.Add(user);

//            var result = handler.Execute(command);

//            result.Errors.Should().HaveCount(1);
//            result.Errors[0].Expression.Should().BeNull();
//            result.Errors[0].Message.Should().Be("Invalid email or password");
//            user.UserEvents.Should().HaveCount(1);
//            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedLogIn);
//        }

//        [TestMethod]
//        public void LogInUserCommandHandler_Fails_For_Invalid_Password() {
//            var handler = new LogInUserCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Active
//            };
//            var command = new LogInUserCommand("test@test.com", "wrong");

//            _playgroundContextFactory.Context.Users.Add(user);

//            var result = handler.Execute(command);

//            result.Errors.Should().HaveCount(1);
//            result.Errors[0].Expression.Should().BeNull();
//            result.Errors[0].Message.Should().Be("Invalid email or password");
//            user.UserEvents.Should().HaveCount(1);
//            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedLogIn);
//        }
//    }
//}