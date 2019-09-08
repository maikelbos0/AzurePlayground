using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Commands.Test {
    [TestClass]
    public class SecurityTests {
        private readonly FakePlaygroundContextFactory _playgroundContextFactory = new FakePlaygroundContextFactory();
        private readonly FakeMailClient _mailClient = new FakeMailClient();
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost" }
            }
        };

        [TestMethod]
        public void RegisterUserCommand_Succeeds() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test"
            };

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();
            _playgroundContextFactory.Context.Users.Should().HaveCount(1);
        }


        [TestMethod]
        public void RegisterUserCommand_Returns_Error_For_Existing_Email() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com"
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Email address already exists");
        }


        [TestMethod]
        public void RegisterUserCommand_Sends_Email() {
            var command = new RegisterUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserRegistration() {
                Email = "test@test.com",
                Password = "test"
            };

            command.Execute(model);

            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void ActivateUserCommand_Succeeds() {
            var command = new ActivateUserCommand(_playgroundContextFactory);
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();
            _playgroundContextFactory.Context.Users.Single().IsActive.Should().BeTrue();
            _playgroundContextFactory.Context.Users.Single().ActivationCode.Should().BeNull();
        }

        [TestMethod]
        public void ActivateUserCommand_Fails_For_Nonexistent_User() {
            var command = new ActivateUserCommand(_playgroundContextFactory);
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test1@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            _playgroundContextFactory.Context.Users.Single().IsActive.Should().BeFalse();
            _playgroundContextFactory.Context.Users.Single().ActivationCode.Should().Be(999999);
        }

        [TestMethod]
        public void ActivateUserCommand_Fails_For_Active_User() {
            var command = new ActivateUserCommand(_playgroundContextFactory);
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = true
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
        }

        [TestMethod]
        public void ActivateUserCommand_Fails_For_Wrong_Code() {
            var command = new ActivateUserCommand(_playgroundContextFactory);
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999987"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.ActivationCode");
            result.Errors[0].Message.Should().Be("This activation code is invalid");
            _playgroundContextFactory.Context.Users.Single().IsActive.Should().BeFalse();
            _playgroundContextFactory.Context.Users.Single().ActivationCode.Should().Be(999999);
        }

        [TestMethod]
        public void SendUserActivationCommand_Sends_Email() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new SendUserActivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _mailClient.SentMessages.Should().HaveCount(1);
            _mailClient.SentMessages[0].Subject.Should().Be("Please activate your account");
            _mailClient.SentMessages[0].To.Should().Be("test@test.com");
        }

        [TestMethod]
        public void SendUserActivationCommand_Creates_New_Activation_Code() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new SendUserActivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _playgroundContextFactory.Context.Users.Single().ActivationCode.Should().NotBe(999999);
        }

        [TestMethod]
        public void SendUserActivationCommand_Does_Nothing_For_Active_User() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new SendUserActivation() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = null,
                IsActive = true
            });

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _playgroundContextFactory.Context.Users.Single().ActivationCode.Should().BeNull();
            _mailClient.SentMessages.Should().HaveCount(0);
        }

        [TestMethod]
        public void SendUserActivationCommand_Does_Nothing_For_Nonexistent_User() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new SendUserActivation() {
                Email = "test1@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                ActivationCode = 999999
            });

            var result = command.Execute(model);

            result.Success.Should().BeTrue();
            _playgroundContextFactory.Context.Users.Single().ActivationCode.Should().Be(999999);
            _mailClient.SentMessages.Should().HaveCount(0);
        }
    }
}