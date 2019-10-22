using AzurePlayground.Database;
using AzurePlayground.Domain.Auditing;
using AzurePlayground.Utilities.Container;

namespace AzurePlayground.Repositories.Auditing {
    [Injectable]
    public sealed class CommandExecutionRepository : ICommandExecutionRepository {
        private readonly IPlaygroundContext _context;

        public CommandExecutionRepository(IPlaygroundContext context) {
            _context = context;
        }

        public void Add(CommandExecution commandExecution) {
            _context.CommandExecutions.Add(commandExecution);
            _context.SaveChanges();
        }
    }
}