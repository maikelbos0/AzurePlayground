//using AzurePlayground.CommandHandlers.Security;
//using AzurePlayground.Commands.Security;
//using AzurePlayground.Domain.Security;
//using AzurePlayground.Test.Utilities;
//using FluentAssertions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Linq;

//namespace AzurePlayground.CommandHandlers.Test.Security {
//    [TestClass]
//    public class ChangeUserPasswordCommandHandlerTests {
//        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

//        [TestMethod]
//        public void ChangeUserPasswordCommandHandler_Succeeds() {
//            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Active
//            };
//            var command = new ChangeUserPasswordCommand("test@test.com", "test", "test2");

//            _playgroundContextFactory.Context.Users.Add(user);

//            var result = handler.Execute(command);

//            result.Success.Should().BeTrue();
//            user.UserEvents.Should().HaveCount(1);
//            user.UserEvents.Single().Type.Should().Be(UserEventType.PasswordChanged);
//            user.Password.Verify("test2").Should().BeTrue();
//        }

//        [TestMethod]
//        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_Nonexistent_User() {
//            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory);
//            var command = new ChangeUserPasswordCommand("test@test.com", "test", "test2");

//            Action commandAction = () => {
//                var result = handler.Execute(command);
//            };

//            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for non-existent user 'test@test.com'");
//        }

//        [TestMethod]
//        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_Inactive_User() {
//            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Inactive
//            };
//            var command = new ChangeUserPasswordCommand("test@test.com", "test", "test2");

//            _playgroundContextFactory.Context.Users.Add(user);

//            Action commandAction = () => {
//                var result = handler.Execute(command);
//            };

//            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for inactive user 'test@test.com'");
//            user.Password.Verify("test").Should().BeTrue();
//        }

//        [TestMethod]
//        public void ChangeUserPasswordCommandHandler_Throws_Exception_For_New_User() {
//            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.New
//            };
//            var command = new ChangeUserPasswordCommand("test@test.com", "test", "test2");

//            _playgroundContextFactory.Context.Users.Add(user);

//            Action commandAction = () => {
//                var result = handler.Execute(command);
//            };

//            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to change password for inactive user 'test@test.com'");
//            user.Password.Verify("test").Should().BeTrue();
//        }

//        [TestMethod]
//        public void ChangeUserPasswordCommandHandler_Fails_For_Wrong_Password() {
//            var handler = new ChangeUserPasswordCommandHandler(_playgroundContextFactory);
//            var user = new User() {
//                Email = "test@test.com",
//                Password = new Password("test"),
//                Status = UserStatus.Active
//            };
//            var command = new ChangeUserPasswordCommand("test@test.com", "wrong", "test2");

//            _playgroundContextFactory.Context.Users.Add(user);

//            var result = handler.Execute(command);

//            result.Errors.Should().HaveCount(1);
//            result.Errors[0].Expression.ToString().Should().Be("p => p.CurrentPassword");
//            result.Errors[0].Message.Should().Be("Invalid password");
//            user.UserEvents.Should().HaveCount(1);
//            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedPasswordChange);
//            user.Password.Verify("test").Should().BeTrue();
//        }
//    }
//}