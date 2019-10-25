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
    public sealed class EditUserInformationCommandTests {

        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;

        public EditUserInformationCommandTests() {
            _repository = new UserRepository(_context);
        }


        [TestMethod]
        public void EditUserInformationCommand_Succeeds() {
            var handler = new EditUserInformationCommandHandler(_repository);
            var command = new EditUserInformationCommand("test@test.com") {
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
        public void EditUserInformationCommand_Throws_Exception_For_Nonexistent_User() {
            var handler = new EditUserInformationCommandHandler(_repository);
            var command = new EditUserInformationCommand("test@test.com") {
                DisplayName = "Test",
                Description = "A test description",
                ShowEmail = true
            };
                        
            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to save user information for non-existent user 'test@test.com'");
        }

        [TestMethod]
        public void EditUserInformationCommand_Throws_Exception_For_New_User() {
            var handler = new EditUserInformationCommandHandler(_repository);
            var command = new EditUserInformationCommand("test@test.com") {
                DisplayName = "Test",
                Description = "A test description",
                ShowEmail = true
            };

            var user = Substitute.For<User>();
            user.Email.Returns("test@test.com");
            user.Status.Returns(UserStatus.New);
            
            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to save user information for inactive user 'test@test.com'");
        }

        [TestMethod]
        public void EditUserInformationCommand_Throws_Exception_For_Inactive_User() {
            var handler = new EditUserInformationCommandHandler(_repository);
            var command = new EditUserInformationCommand("test@test.com") {
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

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to save user information for inactive user 'test@test.com'");
        }
    }
}