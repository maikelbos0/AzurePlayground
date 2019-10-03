namespace AzurePlayground.Database.Migrations {
    using Security = AzurePlayground.Domain.Security;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            SeedUserEventTypes(context);
        }

        private void SeedUserEventTypes(PlaygroundContext context) {
            foreach (var entity in Security.UserEventType.GetValues()) {
                context.UserEventTypes.AddOrUpdate(entity);
            }

            context.SaveChanges();
        }
    }
}
