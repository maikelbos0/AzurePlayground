﻿using AzurePlayground.CommandHandlers.Decorators;
using AzurePlayground.Commands;
using AzurePlayground.Repositories.Auditing;
using AzurePlayground.Test.Utilities;
using AzurePlayground.Utilities.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Tests.Decorators {
    [TestClass]
    public sealed class AuditDecoratorTests {
        public sealed class TestCommand : ICommand {
            public string Test { get; private set; }

            public TestCommand(string test) {
                Test = test;
            }
        }

        public sealed class TestCommandHandler : ICommandHandler<TestCommand> {
            public CommandResult<TestCommand> Execute(TestCommand command) {
                var result = new CommandResult<TestCommand>();

                result.AddError("Error success");

                return result;
            }
        }

        private readonly Serializer _serializer = new Serializer();
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();
        private readonly CommandExecutionRepository _repository;

        public AuditDecoratorTests() {
            _repository = new CommandExecutionRepository(_context);
        }

        [TestMethod]
        public void AuditDecorator_Succeeds() {
            var commandHandler = new TestCommandHandler();
            var command = new TestCommand("Value");
            var decorator = new AuditDecorator<TestCommand>(_repository, _serializer) {
                Handler = commandHandler
            };

            decorator.Execute(command);

            _context.CommandExecutions.Should().HaveCount(1);
            _context.CommandExecutions.First().Date.Should().BeCloseTo(DateTime.UtcNow, 1000);
            _context.CommandExecutions.First().CommandType.Should().Be("AzurePlayground.CommandHandlers.Tests.Decorators.AuditDecoratorTests+TestCommand");
            _context.CommandExecutions.First().CommandData.Should().Be(_serializer.SerializeToJson(command));
        }

        [TestMethod]
        public void AuditDecorator_Calls_Command() {
            var commandHandler = new TestCommandHandler();
            var command = new TestCommand("Value");
            var decorator = new AuditDecorator<TestCommand>(_repository, _serializer) {
                Handler = commandHandler
            };

            var result = decorator.Execute(command);

            result.Errors.Should().HaveCount(1);
            result.Errors.First().Message.Should().Be("Error success");
        }
    }
}