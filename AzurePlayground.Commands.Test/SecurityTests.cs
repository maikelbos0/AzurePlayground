using AzurePlayground.Commands.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AzurePlayground.Commands.Test {
    [TestClass]
    public class SecurityTests {
        private FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private FakeMailClient _mailclient = new FakeMailClient();
        private FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };
        
        [TestMethod]
        public void RegisterUserCommand_Succeeds() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailclient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test"
            };

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();

        }
    }
}