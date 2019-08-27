using AzurePlayground.Domain;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace AzurePlayground.Database {
    public class PlaygroundContext : DbContext {
        public IDbSet<User> Users { get; set; }

        public void DatabaseInitialize() {
            if (!Database.Exists()) {
                Database.Initialize(false);
                DatabaseUpdate();
            }

            if (!Database.CompatibleWithModel(true)) {
                DatabaseUpdate();
            }
        }

        public void DatabaseUpdate() {
            var configuration = new PlaygroundConfiguration();
            var migrator = new DbMigrator(configuration);

            migrator.Update();
        }
    }
}
