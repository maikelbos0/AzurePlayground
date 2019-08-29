namespace AzurePlayground.Database.Migrations {
    using AzurePlayground.Domain;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            var user = context.Users.SingleOrDefault(u => u.Email == "maikel.bos0@gmail.com");

            if (user == null) {
                user = new User() {
                    Email = "maikel.bos0@gmail.com"
                };

                context.Users.Add(user);
            }

            user.UserName = "maikel.bos0";

            context.SaveChanges();
        }
    }
}
