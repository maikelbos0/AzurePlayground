using AzurePlayground.Domain.Auditing;

namespace AzurePlayground.Repositories.Auditing {
    public interface ICommandExecutionRepository {
        void Add(CommandExecution commandExecution);
    }
}