using AzurePlayground.Controllers;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Providers;
using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Unity;

namespace AzurePlayground.Tests.Integration {
    [TestClass]
    public class HomeControllerTests {
        private readonly FakeAuthenticationProvider _authenticationProvider = new FakeAuthenticationProvider();
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestInitialize]
        public void Initialize() {
            UnityConfig.Container.RegisterInstance<IAuthenticationProvider>(_authenticationProvider);
            UnityConfig.Container.RegisterInstance<IPlaygroundContextFactory>(_playgroundContextFactory);
            UnityConfig.Container.RegisterInstance<IMailClient>(_mailClient);
            UnityConfig.Container.RegisterInstance<IAppSettings>(_appSettings);
        }

        private HomeController GetController() {
            return UnityConfig.Container.Resolve<HomeController>();
        }

        [TestMethod]
        public void HomeController_Registration_Activation_To_LogIn_Succeeds() {
            // Registration
            var registrationResult = (ViewResult)GetController().Register(new RegisterUserModel() {
                Email = "test@test.com",
                Password = "test",
                ConfirmPassword = "test"
            });

            registrationResult.ViewName.Should().Be("Registered");
            _mailClient.SentMessages.Should().NotBeEmpty();

            // Activation
            var activationCode = Regex.Match(_mailClient.SentMessages.Last().Body, "\\d+").Value;
            var activationResult = (ViewResult)GetController().Activate(activationCode, "test@test.com");

            activationResult.ViewName.Should().Be("Activated");

            // Log in
            var logInResult = GetController().LogIn(new LogInUserModel() {
                Email = "test@test.com",
                Password = "test"
            });

            logInResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().Be("test@test.com");
        }

        [TestMethod]
        public void HomeController_RequestPasswordReset_ResetPassword_To_LogIn_Succeeds() {
            // Set up
            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Status = UserStatus.Active
            });

            // Request password reset
            var forgotPasswordResult = (ViewResult)GetController().ForgotPassword(new ForgotUserPasswordModel() {
                Email = "test@test.com"
            });

            forgotPasswordResult.ViewName.Should().Be("PasswordResetSent");
            _mailClient.SentMessages.Should().NotBeEmpty();

            // Reset password
            var token = Regex.Match(_mailClient.SentMessages.Last().Body, "(?<=\\=)\\w+(?=\")").Value;
            var resetResult = (ViewResult)GetController().ResetPassword("test@test.com", token, new ResetUserPasswordModel() {
                NewPassword = "test",
                ConfirmNewPassword = "test"
            });

            resetResult.ViewName.Should().Be("PasswordReset");

            // Log in
            var logInResult = GetController().LogIn(new LogInUserModel() {
                Email = "test@test.com",
                Password = "test"
            });

            logInResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().Be("test@test.com");
        }

        [TestMethod]
        public void HomeController_ChangePassword_LogOut_To_LogIn_Succeeds() {
            // Set up
            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            });
            _authenticationProvider.Identity = "test@test.com";

            // Change password
            var changePasswordResult = (ViewResult)GetController().ChangePassword(new ChangeUserPasswordModel() {
                CurrentPassword = "test",
                NewPassword = "hunter2",
                ConfirmNewPassword = "hunter2"
            });
            changePasswordResult.ViewName.Should().Be("PasswordChanged");

            // Log out
            var logOutResult = GetController().LogOut();

            logOutResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().BeNull();

            // Log in
            var logInResult = GetController().LogIn(new LogInUserModel() {
                Email = "test@test.com",
                Password = "hunter2"
            });

            logInResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().Be("test@test.com");
        }

        [TestMethod]
        public void HomeController_FailedLogIn_To_LogIn_To_Deactivate_Succeeds() {
            // Set up
            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                Password = new Password("test"),
                Status = UserStatus.Active
            });

            // Failed log in
            var failedLogInResult = GetController().LogIn(new LogInUserModel() {
                Email = "test@test.com",
                Password = "hunter2"
            });

            failedLogInResult.Should().BeOfType<ViewResult>();
            _authenticationProvider.Identity.Should().BeNull();

            // Log in
            var logInResult = GetController().LogIn(new LogInUserModel() {
                Email = "test@test.com",
                Password = "test"
            });

            logInResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().Be("test@test.com");

            // Deactivate
            var deactivateResult = GetController().Deactivate(new DeactivateUserModel() {
                Password = "test"
            });

            deactivateResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().BeNull();
            _playgroundContextFactory.Context.Users.Single().Status.Should().Be(UserStatus.Inactive);
        }
    }
}