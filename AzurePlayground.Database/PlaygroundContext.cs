﻿using AzurePlayground.Database.Migrations;
using AzurePlayground.Database.ReferenceEntities;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            var userEventTypes = modelBuilder.Entity<UserEventTypeEntity>().ToTable("UserEventTypes", "Security").HasKey(t => t.Id);

            userEventTypes.HasMany(t => t.UserEvents).WithRequired().HasForeignKey(e => e.Type);
            userEventTypes.Property(t => t.Name).IsRequired();

            var userStatus = modelBuilder.Entity<UserStatusEntity>().ToTable("UserStatus", "Security").HasKey(s => s.Id);

            userStatus.HasMany(t => t.Users).WithRequired().HasForeignKey(u => u.Status);
            userStatus.Property(t => t.Name).IsRequired();
            
            var users = modelBuilder.Entity<Security.User>().ToTable("Users", "Security").HasKey(u => u.Id);

            users.HasMany(u => u.UserEvents).WithRequired(e => e.User);
            users.HasIndex(u => u.Email).IsUnique();
            users.Property(u => u.Email).IsRequired().HasMaxLength(255);
            users.Property(u => u.Password.Salt).IsRequired().HasMaxLength(20);
            users.Property(u => u.Password.Hash).IsRequired().HasMaxLength(20);
            users.Property(u => u.Password.HashIterations).IsRequired();
            users.Property(u => u.PasswordResetToken.Salt).HasMaxLength(20);
            users.Property(u => u.PasswordResetToken.Hash).HasMaxLength(20);
            users.Property(u => u.Status).HasColumnName("UserStatus_Id");

            var userEvents = modelBuilder.Entity<Security.UserEvent>().ToTable("UserEvents", "Security").HasKey(e => e.Id);

            userEvents.Property(e => e.Type).HasColumnName("UserEventType_Id");
        }

        public void FixEfProviderServicesProblem() {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
    }
}