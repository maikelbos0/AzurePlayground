﻿using AzurePlayground.Database.Migrations;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Security = AzurePlayground.Domain.Security;

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

        public IDbSet<Security.UserEventType> UserEventTypes { get; set; }

        internal int BaseSaveChanges() {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Security.UserEventType>()
                .ToTable("UserEventTypes", "Security")
                .HasKey(t => t.Id)
                .Property(t => t.Name).IsRequired();
        }

        public override int SaveChanges() {
            foreach (var entity in Security.UserEventType.GetValues()) {
                var entry = Entry(entity);

                if (entry.State == EntityState.Added) {
                    entry.State = EntityState.Unchanged;
                }
            }

            return base.SaveChanges();
        }

        public void FixEfProviderServicesProblem() {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
    }
}