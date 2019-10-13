using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class DeactivateUserCommandTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

        [TestMethod]
        public void DeactivateUserCommand_Succeeds() {
            var command = new DeactivateUserCommand(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var model = new UserDeactivation() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();
            user.Status.Should().Be(UserStatus.Inactive);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.Deactivated);
        }

        [TestMethod]
        public void DeactivateUserCommand_Fails_For_Wrong_Password() {
            var command = new DeactivateUserCommand(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            };
            var model = new UserDeactivation() {
                Email = "test@test.com",
                Password = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Password");
            result.Errors[0].Message.Should().Be("Invalid password");
            user.Status.Should().Be(UserStatus.Active);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedDeactivation);            
        }

        [TestMethod]
        public void DeactivateUserCommand_Throws_Exception_For_Nonexistent_User() {
            var command = new DeactivateUserCommand(_playgroundContextFactory);
            var model = new UserDeactivation() {
                Email = "test@test.com",
                Password = "test"
            };

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate non-existent user 'test@test.com'");
        }

        [TestMethod]
        public void DeactivateUserCommand_Throws_Exception_For_Inactive_User() {
            var command = new DeactivateUserCommand(_playgroundContextFactory);
            var model = new UserDeactivation() {
                Email = "test@test.com",
                Password = "test"
            };

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Status = UserStatus.Inactive
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate inactive user 'test@test.com'");
        }

        [TestMethod]
        public void DeactivateUserCommand_Throws_Exception_For_New_User() {
            var command = new DeactivateUserCommand(_playgroundContextFactory);
            var model = new UserDeactivation() {
                Email = "test@test.com",
                Password = "test"
            };

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Status = UserStatus.New
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate inactive user 'test@test.com'");
        }
    }
}