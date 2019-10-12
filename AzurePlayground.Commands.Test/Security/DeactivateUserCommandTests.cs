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
                IsActive = true
            };
            var model = new UserDeactivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();
            user.IsActive.Should().BeFalse();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.Deactivated);
        }

        [TestMethod]
        public void DeactivateUserCommand_Throws_Exception_For_Nonexistent_User() {
            var command = new DeactivateUserCommand(_playgroundContextFactory);
            var model = new UserDeactivation() {
                Email = "test@test.com"
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
                Email = "test@test.com"
            };

            Action commandAction = () => {
                var result = command.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = false
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate inactive user 'test@test.com'");
        }
    }
}
