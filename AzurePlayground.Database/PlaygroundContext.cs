using AzurePlayground.Database.Migrations;
using AzurePlayground.Domain;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace AzurePlayground.Database {
    public class PlaygroundContext : DbContext {
        static PlaygroundContext() {
            using (var context = new PlaygroundContext()) {
                context.Database.Initialize(false);
            }

            new DbMigrator(new Configuration()).Update();
        }

        public IDbSet<User> Users { get; set; }
    }
}