using AzurePlayground.Database.Migrations;
using AzurePlayground.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace AzurePlayground.Database {
    public class PlaygroundContext : DbContext {
        static PlaygroundContext() {
            // When adding migrations the context can not be instantiated
            // So far I have not worked out how to detect that the model has changed manually
            try {
                using (var context = new PlaygroundContext()) {
                    context.Database.Initialize(false);
                }
            }
            catch (InvalidOperationException) { }

            new DbMigrator(new Configuration()).Update();
        }

        public IDbSet<User> Users { get; set; }
    }
}