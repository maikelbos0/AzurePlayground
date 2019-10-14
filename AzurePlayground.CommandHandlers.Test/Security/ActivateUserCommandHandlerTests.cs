using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Test.Security {
    [TestClass]
    public class ActivateUserCommandHandlerTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();

        [TestMethod]
        public void ActivateUserCommandHandler_Succeeds() {
            var handler = new ActivateUserCommandHandler(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.New
            };
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().BeEmpty();
            user.Status.Should().Be(UserStatus.Active);
            user.ActivationCode.Should().BeNull();
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.Activated);
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Nonexistent_User() {
            var handler = new ActivateUserCommandHandler(_playgroundContextFactory);
            var user = new User() {
                Email = "other@test.com",
                ActivationCode = 999999,
                Status = UserStatus.New
            };
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.Status.Should().Be(UserStatus.New);
            user.ActivationCode.Should().Be(999999);
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Active_User() {
            var handler = new ActivateUserCommandHandler(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.Active
            };
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedActivation);
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Inactive_User() {
            var handler = new ActivateUserCommandHandler(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.Inactive
            };
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.Status.Should().Be(UserStatus.Inactive);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedActivation);
        }

        [TestMethod]
        public void ActivateUserCommandHandler_Fails_For_Wrong_Code() {
            var handler = new ActivateUserCommandHandler(_playgroundContextFactory);
            var user = new User() {
                Email = "test@test.com",
                ActivationCode = 999999,
                Status = UserStatus.New
            };
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999987"
            };

            _playgroundContextFactory.Context.Users.Add(user);

            var result = handler.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            user.Status.Should().Be(UserStatus.New);
            user.ActivationCode.Should().Be(999999);
            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.Single().Type.Should().Be(UserEventType.FailedActivation);
        }
    }
}
