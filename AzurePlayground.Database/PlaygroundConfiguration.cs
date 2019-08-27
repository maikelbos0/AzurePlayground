using AzurePlayground.Domain;
using System.Data.Entity.Migrations;
using System.Linq;

namespace AzurePlayground.Database {
    public class PlaygroundConfiguration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            base.Seed(context);

            if (!context.Users.Any()) {
                context.Users.Add(new User() {
                    Email = "maikel.bos0@gmail.com"
                });

                context.SaveChanges();
            }
        }
    }
}
