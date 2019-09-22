using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Commands.Test.Security {
    [TestClass]
    public class RequestUserPasswordResetCommandTests {
        private readonly byte[] _passwordHash = new byte[] { 248, 212, 57, 28, 32, 158, 38, 248, 82, 175, 53, 217, 161, 238, 108, 226, 48, 123, 118, 173 };
        private readonly int _passwordHashIterations = 1000;
        private readonly byte[] _passwordSalt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void RequestUserPasswordResetCommand_Sends_Email() {
            var command = new RequestUserPasswordResetCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserPasswordResetRequest() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true
            });

            command.Execute(model);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Your password reset request");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void RequestUserPasswordResetCommand_Succeeds() {
            var command = new RequestUserPasswordResetCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserPasswordResetRequest() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = true
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Success.Should().BeTrue();
            user.PasswordResetTokenSalt.Should().NotBeNull();
            user.PasswordResetTokenHash.Should().NotBeNull();
            user.PasswordResetTokenHashIterations.Should().NotBeNull();
            user.PasswordResetTokenExpiryDate.Should().NotBeNull();
        }

        [TestMethod]
        public void RequestUserPasswordResetCommand_Does_Nothing_For_Nonexistent_User() {
            var command = new RequestUserPasswordResetCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserPasswordResetRequest() {
                Email = "test@test.com"
            };

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void RequestUserPasswordResetCommand_Does_Nothing_For_Inactive_User() {
            var command = new RequestUserPasswordResetCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserPasswordResetRequest() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = _passwordHash,
                PasswordHashIterations = _passwordHashIterations,
                PasswordSalt = _passwordSalt,
                IsActive = false
            });

            var result = command.Execute(model);
            var user = _playgroundContextFactory.Context.Users.Single();

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().BeEmpty();
            user.PasswordResetTokenSalt.Should().BeNull();
            user.PasswordResetTokenHash.Should().BeNull();
            user.PasswordResetTokenHashIterations.Should().BeNull();
            user.PasswordResetTokenExpiryDate.Should().BeNull();
        }
    }
}
