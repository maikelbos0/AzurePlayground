﻿using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AzurePlayground.Utilities.Tests.Mail {
    [TestClass]
    public sealed class ConfirmEmailMailTemplateTests {
        private readonly FakeAppSettings _appSettings = new FakeAppSettings() {
            Settings = new Dictionary<string, string>() {
                { "Application.BaseUrl", "http://localhost/" }
            }
        };

        [TestMethod]
        public void ConfirmEmailMailTemplate_Generates_Correct_Subject() {
            var template = new ConfirmEmailMailTemplate(_appSettings);
            var parameters = new ConfirmEmailMailTemplateParameters("new@test.com", 12345);

            template.GetMessage(parameters, "test@test.com").Subject.Should().Be("Please confirm your email address");
        }

        [TestMethod]
        public void ConfirmEmailMailTemplate_Generates_Correct_Body() {
            var template = new ConfirmEmailMailTemplate(_appSettings);
            var parameters = new ConfirmEmailMailTemplateParameters("new@test.com", 12345);

            template.GetMessage(parameters, "test@test.com").Body.Should().Be("<p>Please <a href=\"http://localhost/Home/ConfirmEmail/?confirmationCode=12345&email=new%40test.com\">click here to confirm your new email address</a>.</p>");
        }

        [TestMethod]
        public void ConfirmEmailMailTemplate_Uses_Correct_Address() {
            var template = new ConfirmEmailMailTemplate(_appSettings);
            var parameters = new ConfirmEmailMailTemplateParameters("new@test.com", 12345);

            template.GetMessage(parameters, "test@test.com").To.Should().Be("test@test.com");
        }
    }
}