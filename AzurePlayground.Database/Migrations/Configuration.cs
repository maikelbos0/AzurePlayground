namespace AzurePlayground.Database.Migrations {
    using AzurePlayground.Domain;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AzurePlayground.Database.PlaygroundContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AzurePlayground.Database.PlaygroundContext context) {
            var user = context.Users.SingleOrDefault(u => u.Email == "maikel.bos0@gmail.com") ?? new User() {
                Email = "maikel.bos0@gmail.com"
            };

            user.UserName = "maikel.bos0";

            context.Users.AddOrUpdate(user);
            context.SaveChanges();
        }
    }
}
