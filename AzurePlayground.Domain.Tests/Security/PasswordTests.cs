using AzurePlayground.Domain.Security;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.Domain.Tests.Security {
    [TestClass]
    public class PasswordTests {
        [TestMethod]
        public void Password_Generates_Unique_Salt() {
            var pass1 = new Password("test");
            var pass2 = new Password("test");

            pass1.Salt.SequenceEqual(pass2.Salt).Should().BeFalse();
        }

        [TestMethod]
        public void Passwords_With_Same_Properties_Are_Equal() {
            var pass1 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);
            var pass2 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);

            pass1.Should().Be(pass2);
        }

        [TestMethod]
        public void Passwords_With_Different_Salts_Are_Unequal() {
            var pass1 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);
            var pass2 = CreatePassword(new byte[] { 1, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);

            pass1.Should().NotBe(pass2);
        }

        [TestMethod]
        public void Passwords_With_Different_Hashes_Are_Unequal() {
            var pass1 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);
            var pass2 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 11, 11, 13, 14, 16 }, 1000, null);

            pass1.Should().NotBe(pass2);
        }

        [TestMethod]
        public void Passwords_With_Different_Iterations_Are_Unequal() {
            var pass1 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);
            var pass2 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 2000, null);

            pass1.Should().NotBe(pass2);
        }

        [TestMethod]
        public void Passwords_With_Different_ExpiryDates_Are_Unequal() {
            var pass1 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, new DateTime(1970, 1, 1));
            var pass2 = CreatePassword(new byte[] { 0, 1, 3, 4, 6 }, new byte[] { 10, 11, 13, 14, 16 }, 1000, null);

            pass1.Should().NotBe(pass2);
        }

        private Password CreatePassword(byte[] salt, byte[] hash, int iterations, DateTime? expiryDate) {
            var password = (Password)Activator.CreateInstance(typeof(Password), true);

            typeof(Password).GetProperty("Salt").SetValue(password, salt);
            typeof(Password).GetProperty("Hash").SetValue(password, hash);
            typeof(Password).GetProperty("HashIterations").SetValue(password, iterations);
            typeof(Password).GetProperty("ExpiryDate").SetValue(password, expiryDate);

            return password;
        }
    }
}