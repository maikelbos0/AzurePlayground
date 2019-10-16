﻿using AzurePlayground.Services;
using AzurePlayground.Commands;
using AzurePlayground.CommandHandlers;
using AzurePlayground.Queries;
using AzurePlayground.QueryHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzurePlayground.Utilities.Container;
using FluentAssertions;

namespace AzurePlayground.Tests.Services {
    [TestClass]
    public class MessageServiceTests {
        public class TestCommand : ICommand {
        }

        [Injectable]
        public class TestCommandHandler : ICommandHandler<TestCommand> {
            public CommandResult<TestCommand> Execute(TestCommand command) {
                return new CommandResult<TestCommand>();
            }
        }

        public class TestQuery : IQuery<bool> {
        }

        [Injectable]
        public class TestQueryHander : IQueryHandler<TestQuery, bool> {
            public bool Execute(TestQuery query) {
                return true;
            }
        }


        [TestMethod]
        public void MessageService_Dispatches_Command_To_CommandHandler() {
            var messageService = new MessageService(UnityConfig.Container);
            var command = new TestCommand();

            var result = messageService.Dispatch(command);

            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void MessageService_Dispatches_Query_To_QueryHandler() {
            var messageService = new MessageService(UnityConfig.Container);
            var query = new TestQuery();

            var result = messageService.Dispatch(query);

            result.Should().BeTrue();
        }
    }
}