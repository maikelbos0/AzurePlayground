using AzurePlayground.Domain.Security;
using AzurePlayground.Queries.Security;
using AzurePlayground.QueryHandlers.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AzurePlayground.QueryHandlers.Tests.Security {
    [TestClass]
    public sealed class GetUserProfileQueryHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();

        [TestMethod]
        public void GetUserProfileQueryHandler_Succeeds() {
            var handler = new GetUserProfileQueryHandler(_context);
            var command = new GetUserProfileQuery("test@test.com");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.DisplayName.Returns("Test name");
            user.Description.Returns("Test description");
            user.ShowEmail.Returns(true);

            _context.Users.Add(user);

            var result = handler.Execute(command);

            result.DisplayName.Should().Be("Test name");
            result.Description.Should().Be("Test description");
            result.ShowEmail.Should().BeTrue();
        }

        [TestMethod]
        public void GetUserProfileQueryHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new GetUserProfileQueryHandler(_context);
            var command = new GetUserProfileQuery("test@test.com");

            Action queryAction = () => {
                var result = handler.Execute(command);
            };

            queryAction.Should().Throw<InvalidOperationException>();
        }
    }
}