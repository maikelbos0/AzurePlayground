using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.Registered);
        }

        [TestMethod]
        public void RegisterUserCommand_Fails_For_Existing_Email() {
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
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.Activated);
        }

        [TestMethod]
        public void ActivateUserCommand_Fails_For_Nonexistent_User() {
            var command = new ActivateUserCommand(_playgroundContextFactory);
            var model = new UserActivation() {
                Email = "test@test.com",
                ActivationCode = "999999"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "other@test.com",
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
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedActivation);
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
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedActivation);
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
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.ActivationCodeSent);
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
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(0);
            _mailClient.SentMessages.Should().HaveCount(0);
        }

        [TestMethod]
        public void SendUserActivationCommand_Does_Nothing_For_Nonexistent_User() {
            var command = new SendUserActivationCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new SendUserActivation() {
                Email = "other@test.com"
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

        [TestMethod]
        public void LogInUserCommand_Succeeds() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = new byte[] { 248, 212, 57, 28, 32, 158, 38, 248, 82, 175, 53, 217, 161, 238, 108, 226, 48, 123, 118, 173 },
                PasswordHashIterations = 1000,
                PasswordSalt = Enumerable.Range(0, 20).Select(i => (byte)0).ToArray(),
                IsActive = true
            });

            var result = command.Execute(model);

            result.Errors.Should().BeEmpty();
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.LoggedIn);
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Nonexistent_User() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "other@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = new byte[] { 248, 212, 57, 28, 32, 158, 38, 248, 82, 175, 53, 217, 161, 238, 108, 226, 48, 123, 118, 173 },
                PasswordHashIterations = 1000,
                PasswordSalt = Enumerable.Range(0, 20).Select(i => (byte)0).ToArray(),
                IsActive = true
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Inactive_User() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "test"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = new byte[] { 248, 212, 57, 28, 32, 158, 38, 248, 82, 175, 53, 217, 161, 238, 108, 226, 48, 123, 118, 173 },
                PasswordHashIterations = 1000,
                PasswordSalt = Enumerable.Range(0, 20).Select(i => (byte)0).ToArray()
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedLogIn);
        }

        [TestMethod]
        public void LogInUserCommand_Fails_For_Invalid_Password() {
            var command = new LogInUserCommand(_playgroundContextFactory, _mailClient, _appSettings);
            var model = new UserLogIn() {
                Email = "test@test.com",
                Password = "wrong"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                PasswordHash = new byte[] { 248, 212, 57, 28, 32, 158, 38, 248, 82, 175, 53, 217, 161, 238, 108, 226, 48, 123, 118, 173 },
                PasswordHashIterations = 1000,
                PasswordSalt = Enumerable.Range(0, 20).Select(i => (byte)0).ToArray(),
                IsActive = true
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Expression.ToString().Should().Be("p => p.Email");
            result.Errors[0].Message.Should().Be("Invalid email or password");
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.FailedLogIn);
        }

        [TestMethod]
        public void LogOutUserCommand_Succeeds() {
            var command = new LogOutUserCommand(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = true
            });

            var result = command.Execute(model);

            result.Errors.Should().HaveCount(0);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Should().HaveCount(1);
            _playgroundContextFactory.Context.Users.Single().UserEvents.Single().UserEventType.Should().Be(UserEventType.LoggedOut);
        }

        [TestMethod]
        public void LogOutUserCommand_Throws_Exception_For_Inactive_User() {
            var command = new LogOutUserCommand(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = command.Execute(model);
            };

            _playgroundContextFactory.Context.Users.Add(new User() {
                Email = "test@test.com",
                IsActive = false
            });

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out inactive user 'test@test.com'");
        }

        [TestMethod]
        public void LogOutUserCommand_Throws_Exception_For_Nonexistent_User() {
            var command = new LogOutUserCommand(_playgroundContextFactory);
            var model = new UserLogOut() {
                Email = "test@test.com"
            };
            Action commandAction = () => {
                var result = command.Execute(model);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to log out non-existent user 'test@test.com'");
        }
    }
}