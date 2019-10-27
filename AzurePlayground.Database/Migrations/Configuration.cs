namespace AzurePlayground.Database.Migrations {
    using AzurePlayground.Database.ReferenceEntities;
    using AzurePlayground.Domain.Security;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<PlaygroundContext> {
        protected override void Seed(PlaygroundContext context) {
            SeedEntityType<UserEventType, UserEventTypeEntity>(context);
            SeedEntityType<UserStatus, UserStatusEntity>(context);

            AddOrUpdateUser(context, "example@test.com", "I am example", null, true);
            AddOrUpdateUser(context, "anon@test.com", null, null, false);
            AddOrUpdateUser(context, "you@test.com", "You", "Who are you?", false);
            AddOrUpdateUser(context, "another@test.com", "Another", "One bites the dust", true);
        }

        private void SeedEntityType<TEnum, TReferenceEntity>(PlaygroundContext context)
            where TEnum : Enum
            where TReferenceEntity : BaseReferenceEntity<TEnum>, new() {

            foreach (var entity in ReferenceEntityExtensions.GetValues<TEnum, TReferenceEntity>()) {
                context.Set<TReferenceEntity>().AddOrUpdate(entity);
            }

            context.SaveChanges();
        }

        private void AddOrUpdateUser(PlaygroundContext context, string email, string displayName, string description, bool showEmail) {
            var user = context.Users.SingleOrDefault(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

            if (user == null) {
                user = new User(email, new Random().Next().ToString());
                context.Users.Add(user);
            }

            user.Activate();
            user.DisplayName = displayName;
            user.Description = description;
            user.ShowEmail = showEmail;

            context.SaveChanges();
        }
    }
}