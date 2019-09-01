using AzurePlayground.Database.Migrations;
using Security = AzurePlayground.Domain.Security;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace AzurePlayground.Database {
    public class PlaygroundContext : DbContext, IPlaygroundContext {
        static PlaygroundContext() {
            // When adding migrations the context can not be instantiated
            // So far I have not worked out how to detect that the model has changed manually
            try {
                using (var context = new PlaygroundContext()) {
                    context.Database.Initialize(false);
                }
            }
            catch (Exception) { }

            // The update command also tries to instantiate the context
            try {
                new DbMigrator(new Configuration()).Update();
            }
            catch (Exception) { }
        }

        public IDbSet<Security.User> Users { get; set; }

        public void FixEfProviderServicesProblem() {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}