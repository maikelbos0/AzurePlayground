using AzurePlayground.Database;
using Security = AzurePlayground.Domain.Security;
using System.Data.Entity;

namespace AzurePlayground.Test.Utilities {
    public class FakePlaygroundContext : IPlaygroundContext {
        public IDbSet<Security.User> Users { get; set; } = new FakeDbSet<Security.User>();

        public void Dispose() {
            SaveChanges();
        }

        public int SaveChanges() {
            // For now, implementation is not needed
            return 0;
        }
    }
}