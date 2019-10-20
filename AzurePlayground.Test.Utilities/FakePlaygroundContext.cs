using AzurePlayground.Database;
using System.Data.Entity;
using Security = AzurePlayground.Domain.Security;

namespace AzurePlayground.Test.Utilities {
    public class FakePlaygroundContext : IPlaygroundContext {
        public IDbSet<Security.User> Users { get; set; } = new FakeDbSet<Security.User>();
        public int CallsToSaveChanges { get; private set; }

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