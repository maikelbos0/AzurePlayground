namespace AzurePlayground.Database.Migrations {
    using AzurePlayground.Database.ReferenceEntities;
    using AzurePlayground.Domain.Security;
    using System;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            SeedEntityType<UserEventType, UserEventTypeEntity>(context);
            SeedEntityType<UserStatus, UserStatusEntity>(context);
        }

        private void SeedEntityType<TEnum, TReferenceEntity>(PlaygroundContext context)
            where TEnum : Enum
            where TReferenceEntity : BaseReferenceEntity<TEnum>, new() {

            foreach (var entity in ReferenceEntityExtensions.GetValues<TEnum, TReferenceEntity>()) {
                context.Set<TReferenceEntity>().AddOrUpdate(entity);
            }

            context.SaveChanges();
        }
    }
}