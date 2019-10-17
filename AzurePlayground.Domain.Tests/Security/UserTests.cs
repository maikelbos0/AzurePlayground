using AzurePlayground.Domain.Security;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.Domain.Tests.Security {
    [TestClass]
    public class UserTests {
        [TestMethod]
        public void User_AddEvent_Adds_Event_Correctly() {
            var user = new User();

            user.AddEvent(UserEventType.FailedLogIn);

            user.UserEvents.Should().HaveCount(1);
            user.UserEvents.First().Type.Should().Be(UserEventType.FailedLogIn);
            user.UserEvents.First().Date.Should().BeCloseTo(DateTime.UtcNow);
        }
    }
}
