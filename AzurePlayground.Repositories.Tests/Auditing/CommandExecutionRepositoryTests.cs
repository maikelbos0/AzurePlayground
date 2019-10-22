using AzurePlayground.Domain.Auditing;
using AzurePlayground.Repositories.Auditing;
using AzurePlayground.Test.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzurePlayground.Repositories.Tests.Auditing {
    [TestClass]
    public class CommandExecutionRepositoryTests {
        private readonly FakePlaygroundContext _context = new FakePlaygroundContext();

        [TestMethod]
        public void CommandExecutionRepository_Add_Succeeds() {
            var repository = new CommandExecutionRepository(_context);
            var execution = new CommandExecution("test", "test");

            repository.Add(execution);

            _context.CommandExecutions.Should().Contain(execution);
        }

        [TestMethod]
        public void CommandExecutionRepository_Add_Saves() {
            var repository = new CommandExecutionRepository(_context);

            repository.Add(new CommandExecution("test", "test"));

            _context.CallsToSaveChanges.Should().Be(1);
        }
    }
}
