using AzurePlayground.Domain;
using System.Data.Entity;

namespace AzurePlayground.Database {
    public class PlaygroundContext : DbContext {
        public IDbSet<User> Users { get; set; }
    }
}