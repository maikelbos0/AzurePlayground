namespace AzurePlayground.Database.Migrations {
    using AzurePlayground.Database.ReferenceEntities;
    using AzurePlayground.Domain.Security;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            SeedUserEventTypes(context);
        }

        private void SeedUserEventTypes(PlaygroundContext context) {
            foreach (var entity in ReferenceEntityExtensions.GetValues<UserEventType, UserEventTypeEntity>()) {
                context.Set<UserEventTypeEntity>().AddOrUpdate(entity);
            }

            foreach (var entity in ReferenceEntityExtensions.GetValues<UserStatus, UserStatusEntity>()) {
                context.Set<UserStatusEntity>().AddOrUpdate(entity);
            }

            context.SaveChanges();
        }
    }
}