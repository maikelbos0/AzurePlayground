using AzurePlayground.Database;
using System.Data.Entity;
using Security = AzurePlayground.Domain.Security;
using Auditing = AzurePlayground.Domain.Auditing;

namespace AzurePlayground.Test.Utilities {
    public sealed class FakePlaygroundContext : IPlaygroundContext {
        public IDbSet<Security.User> Users { get; set; } = new FakeDbSet<Security.User>();
        public int CallsToSaveChanges { get; private set; }
        public IDbSet<Auditing.CommandExecution> CommandExecutions { get ; set ; } = new FakeDbSet<Auditing.CommandExecution>();

        public void Dispose() {
            SaveChanges();
        }

        public int SaveChanges() {
            CallsToSaveChanges++;

            // For now, implementation is not needed
            return 0;
        }
    }
}