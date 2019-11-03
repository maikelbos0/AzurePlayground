using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public sealed class ChangeUserProfileCommandTests {

        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;

        public ChangeUserProfileCommandTests() {
            _repository = new UserRepository(_context);
        }


        [TestMethod]
        public void ChangeUserProfileCommandHandler_Succeeds() {
            var handler = new ChangeUserProfileCommandHandler(_repository);
            var command = new ChangeUserProfileCommand("test@test.com") {
                DisplayName = "Test",
                Description = "A test description",
                ShowEmail = true
            };

            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Active);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.Errors.Should().BeEmpty();
            user.DisplayName.Should().Be("Test");
            user.Description.Should().Be("A test description");
            user.ShowEmail.Should().BeTrue();
        }

        [TestMethod]
        public void ChangeUserProfileCommandHandler_Throws_Exception_For_Invalid_User() {
            var handler = new ChangeUserProfileCommandHandler(_repository);
            var command = new ChangeUserProfileCommand("test@test.com") {
                DisplayName = "Test",
                Description = "A test description",
                ShowEmail = true
            };

            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.Inactive);
            
            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>();
        }
    }
}