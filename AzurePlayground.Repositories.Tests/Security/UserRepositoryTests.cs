﻿using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AzurePlayground.Repositories.Tests.Security {
    [TestClass]
    public sealed class UserRepositoryTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();

        [TestInitialize]
        public void Initialize() {
            _context.Users.Add(new User("first@test.com", "test"));
            _context.Users.Add(new User("test@test.com", "test"));
            _context.Users.Add(new User("ANOTHER@TEST.COM", "test"));
            _context.Users.Add(new User("wrong@test.com", "test"));
            _context.Users.Add(new User("accent@test.com", "test"));
            _context.Users.Add(new User("áççënt@test.com", "test"));
            _context.Users.Add(new User("inactive@test.com", "test"));

            var id = 1;

            foreach (var user in _context.Users) {
                if (!user.Email.Equals("inactive@test.com", StringComparison.InvariantCultureIgnoreCase)) {
                    user.Activate();
                }

                typeof(User).GetProperty("Id").SetValue(user, id++);
            }
        }

        [TestMethod]
        public void UserRepository_TryGetByEmail_Succeeds() {
            var repository = new UserRepository(_context);

            var user = repository.TryGetByEmail("test@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("test@test.com");
        }

        [TestMethod]
        public void UserRepository_TryGetByEmail_Is_Case_Insensitive() {
            var repository = new UserRepository(_context);

            var user = repository.TryGetByEmail("another@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("ANOTHER@TEST.COM");
        }

        [TestMethod]
        public void UserRepository_TryGetByEmail_Is_Accent_Sensitive() {
            var repository = new UserRepository(_context);

            var user = repository.TryGetByEmail("áççënt@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("áççënt@test.com");
        }

        [TestMethod]
        public void UserRepository_TryGetByEmail_Returns_Null_For_Nonexistent_Email() {
            var repository = new UserRepository(_context);

            var user = repository.TryGetByEmail("nobody@test.com");

            user.Should().BeNull();
        }

        [TestMethod]
        public void UserRepository_TryGetByEmail_Does_Not_Save() {
            var repository = new UserRepository(_context);

            repository.TryGetByEmail("test@test.com");

            _context.CallsToSaveChanges.Should().Be(0);
        }

        [TestMethod]
        public void UserRepository_GetActiveByEmail_Succeeds() {
            var repository = new UserRepository(_context);

            var user = repository.GetActiveByEmail("test@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("test@test.com");
        }

        [TestMethod]
        public void UserRepository_GetActiveByEmail_Is_Case_Insensitive() {
            var repository = new UserRepository(_context);

            var user = repository.GetActiveByEmail("another@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("ANOTHER@TEST.COM");
        }

        [TestMethod]
        public void UserRepository_GetActiveByEmail_Is_Accent_Sensitive() {
            var repository = new UserRepository(_context);

            var user = repository.GetActiveByEmail("áççënt@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("áççënt@test.com");
        }

        [TestMethod]
        public void UserRepository_GetActiveByEmail_Throws_Exception_For_Nonexistent_Email() {
            var repository = new UserRepository(_context);

            Action action = () => repository.GetActiveByEmail("nobody@test.com");

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void UserRepository_GetActiveByEmail_Throws_Exception_For_Wrong_Status() {
            var repository = new UserRepository(_context);

            Action action = () => repository.GetActiveByEmail("inactive@test.com");

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void UserRepository_GetActiveByEmail_Does_Not_Save() {
            var repository = new UserRepository(_context);

            repository.GetActiveByEmail("test@test.com");

            _context.CallsToSaveChanges.Should().Be(0);
        }

        [TestMethod]
        public void UserRepository_Add_Succeeds() {
            var repository = new UserRepository(_context);
            var user = new User("new@test.com", "test");

            repository.Add(user);

            _context.Users.Should().Contain(user);
        }

        [TestMethod]
        public void UserRepository_Add_Saves() {
            var repository = new UserRepository(_context);

            repository.Add(new User("new@test.com", "test"));

            _context.CallsToSaveChanges.Should().Be(1);
        }

        [TestMethod]
        public void UserRepository_Update_Saves() {
            var repository = new UserRepository(_context);

            repository.Update();

            _context.CallsToSaveChanges.Should().Be(1);
        }
    }
}