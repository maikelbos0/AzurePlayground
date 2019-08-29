using AzurePlayground.Database.Migrations;
using AzurePlayground.Domain;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace AzurePlayground.Database {
    public class PlaygroundContext : DbContext {
        public IDbSet<User> Users { get; set; }

        public void DatabaseInitialize() {
            Database.Initialize(false);

            new DbMigrator(new Configuration()).Update();
        }
    }
}