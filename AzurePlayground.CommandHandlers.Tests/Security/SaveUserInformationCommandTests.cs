using AzurePlayground.CommandHandlers.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AzurePlayground.CommandHandlers.Tests.Security {
    [TestClass]
    public sealed class SaveUserInformationCommandTests {

        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly UserRepository _repository;

        public SaveUserInformationCommandTests() {
            _repository = new UserRepository(_context);
        }


        [TestMethod]
        public void SaveUserInformationCommand_Succeeds() {
            var handler = new SaveUserInformationCommandHandler(_repository);
            var command = new SaveUserInformationCommand("test@test.com") {
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
        public void SaveUserInformationCommand_Throws_Exception_For_Nonexistent_User() {
            throw new System.NotImplementedException();

            /*
            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate non-existent user 'test@test.com'");
            */
        }

        [TestMethod]
        public void SaveUserInformationCommand_Throws_Exception_For_Inactive_User() {
            throw new System.NotImplementedException();

            /*
            Action commandAction = () => {
                var result = handler.Execute(command);
            };

            _context.Users.Add(user);

            commandAction.Should().Throw<InvalidOperationException>().WithMessage("Attempted to deactivate inactive user 'test@test.com'");
            */
        }
    }
}