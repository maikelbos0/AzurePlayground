using AzurePlayground.Domain.Security;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.Domain.Tests.Security {
    [TestClass]
    public class UserTests {
        [TestMethod]
        public void User_Register_Succeeds() {
            var user = new User("test@test.com", "test");

            user.ActivationCode.Should().NotBeNull();
            user.Status.Should().Be(UserStatus.New);
            user.UserEvents.Last().Type.Should().Be(UserEventType.Registered);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_LogIn_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.GeneratePasswordResetToken();
            user.LogIn();

            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
            user.UserEvents.Last().Type.Should().Be(UserEventType.LoggedIn);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_LogInFailed_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.LogInFailed();

            user.UserEvents.Last().Type.Should().Be(UserEventType.FailedLogIn);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_LogOut_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.LogOut();

            user.UserEvents.Last().Type.Should().Be(UserEventType.LoggedOut);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_GeneratePasswordResetToken_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();

            var token = user.GeneratePasswordResetToken();

            user.PasswordResetToken.Verify(token).Should().BeTrue();
            user.UserEvents.Last().Type.Should().Be(UserEventType.PasswordResetRequested);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_Activate_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();

            user.ActivationCode.Should().BeNull();
            user.Status.Should().Be(UserStatus.Active);
            user.UserEvents.Last().Type.Should().Be(UserEventType.Activated);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_ActivationFailed_Succeeds() {
            var user = new User("test@test.com", "test");

            user.ActivationFailed();

            user.ActivationCode.Should().NotBeNull();
            user.Status.Should().Be(UserStatus.New);
            user.UserEvents.Last().Type.Should().Be(UserEventType.FailedActivation);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_ChangePassword_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.ChangePassword("new");

            user.Password.Verify("new").Should().BeTrue();
            user.UserEvents.Last().Type.Should().Be(UserEventType.PasswordChanged);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_ChangePasswordFailed_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.ChangePasswordFailed();

            user.UserEvents.Last().Type.Should().Be(UserEventType.FailedPasswordChange);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_ResetPassword_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.GeneratePasswordResetToken();
            user.ResetPassword("new");

            user.PasswordResetToken.Should().Be(TemporaryPassword.None);
            user.Password.Verify("new").Should().BeTrue();
            user.UserEvents.Last().Type.Should().Be(UserEventType.PasswordReset);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_ResetPasswordFailed_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.ResetPasswordFailed();

            user.UserEvents.Last().Type.Should().Be(UserEventType.FailedPasswordReset);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_GenerateActivationCode_Succeeds() {
            var user = new User("test@test.com", "test");
            var oldActivationCode = user.ActivationCode;
            
            user.GenerateActivationCode();

            user.ActivationCode.Should().NotBe(oldActivationCode); // This test will fail once every 256 ^ 3 runs
            user.UserEvents.Last().Type.Should().Be(UserEventType.ActivationCodeSent);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void User_Deactivate_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.Deactivate();

            user.Status.Should().Be(UserStatus.Inactive);
            user.UserEvents.Last().Type.Should().Be(UserEventType.Deactivated);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod] public void User_DeactivationFailed_Succeeds() {
            var user = new User("test@test.com", "test");

            user.Activate();
            user.DeactivationFailed();

            user.UserEvents.Last().Type.Should().Be(UserEventType.FailedDeactivation);
            user.UserEvents.Last().Date.Should().BeCloseTo(DateTime.UtcNow);
        }
    }
}