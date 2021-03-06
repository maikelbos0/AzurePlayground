﻿using AzurePlayground.Domain.Security;
using AzurePlayground.Queries.Security;
using AzurePlayground.QueryHandlers.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AzurePlayground.QueryHandlers.Tests.Security {
    [TestClass]
    public sealed class GetUserNewEmailQueryHandlerTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();

        [TestMethod]
        public void GetUserNewEmailQueryHandler_Gives_NewEmail_If_Available() {
            var handler = new GetUserNewEmailQueryHandler(_context);
            var query = new GetUserNewEmailQuery("test@test.com");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.NewEmail.Returns("new@test.com");

            _context.Users.Add(user);

            var result = handler.Execute(query);

            result.Should().Be("new@test.com");
        }

        [TestMethod]
        public void GetUserNewEmailQueryHandler_Gives_Null_NewEmail_If_NewEmail_Is_Not_Available() {
            var handler = new GetUserNewEmailQueryHandler(_context);
            var query = new GetUserNewEmailQuery("test@test.com");
            var user = Substitute.For<User>();

            user.Email.Returns("test@test.com");
            user.NewEmail.Returns((string)null);

            _context.Users.Add(user);

            var result = handler.Execute(query);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetUserNewEmailQueryHandler_Throws_Exception_For_Nonexistent_User() {
            var handler = new GetUserNewEmailQueryHandler(_context);
            var query = new GetUserNewEmailQuery("test@test.com");

            Action queryAction = () => {
                var result = handler.Execute(query);
            };

            queryAction.Should().Throw<InvalidOperationException>();
        }
    }
}