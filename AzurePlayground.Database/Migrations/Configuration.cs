namespace AzurePlayground.Database.Migrations {
    using Security = AzurePlayground.Domain.Security;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            var user = context.Users.SingleOrDefault(u => u.Email == "maikel.bos0@gmail.com");

            if (user == null) {
                user = new Security.User() {
                    Email = "maikel.bos0@gmail.com"
                };

                context.Users.Add(user);
            }

            user.PasswordSalt = new byte[] { 42, 104, 216, 106, 1, 135, 109, 59, 174, 205, 27, 21, 128, 63, 234, 59, 120, 114, 37, 60 };
            user.PasswordHash = new byte[] { 75, 42, 235, 31, 240, 251, 146, 23, 44, 51, 254, 147, 91, 240, 69, 43, 163, 195, 34, 11 };
            user.PasswordHashIterations = 1000;
            user.IsActive = true;

            context.SaveChanges();
        }
    }
}
