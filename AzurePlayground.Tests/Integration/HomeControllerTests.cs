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
using System;
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

        [TestMethod]
        public void HomeController_Registration_Activation_To_LogIn_Succeeds() {
            var controller = UnityConfig.Container.Resolve<HomeController>();

            // Registration
            var registrationResult = (ViewResult)controller.Register(new UserRegistration() {
                Email = "test@test.com",
                Password = "test",
                ConfirmPassword = "test"
            });

            registrationResult.ViewName.Should().Be("Registered");
            _mailClient.SentMessages.Should().NotBeEmpty();

            // Activation
            var activationCode = Regex.Match(_mailClient.SentMessages.Last().Body, "\\d+").Value;
            var activationResult = (ViewResult)controller.Activate(activationCode, "test@test.com");

            activationResult.ViewName.Should().Be("Activated");

            // Log in
            var logInResult = controller.LogIn(new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            });

            logInResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().Be("test@test.com");
        }

        [TestMethod]
        public void HomeController_RequestPasswordReset_ResetPassword_To_LogIn_Succeeds() {
            var controller = UnityConfig.Container.Resolve<HomeController>();

            // Set up user
            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = true
            });

            // Request password reset
            var forgotPasswordResult = (ViewResult)controller.ForgotPassword(new UserPasswordResetRequest() {
                Email = "test@test.com"
            });

            forgotPasswordResult.ViewName.Should().Be("PasswordResetSent");
            _mailClient.SentMessages.Should().NotBeEmpty();

            // Reset password
            var token = Regex.Match(_mailClient.SentMessages.Last().Body, "(?<=\\=)\\w+(?=\")").Value;
            var resetResult = (ViewResult)controller.ResetPassword(new UserPasswordReset() {
                Email = "test@test.com",
                PasswordResetToken = token,
                NewPassword = "test",
                ConfirmNewPassword = "test"
            });

            resetResult.ViewName.Should().Be("PasswordReset");

            // Log in
            var logInResult = controller.LogIn(new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            });

            logInResult.Should().BeOfType<RedirectToRouteResult>();
            _authenticationProvider.Identity.Should().Be("test@test.com");
        }
    }
}