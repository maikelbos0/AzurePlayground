using AzurePlayground.Controllers;
using AzurePlayground.Database;
using AzurePlayground.Models.Security;
using AzurePlayground.Providers;
using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var registerModel = new UserRegistration() {
                Email = "test@test.com",
                Password = ""
            };

            throw new NotImplementedException();
        }
    }
}