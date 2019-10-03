using System;
using System.Data.Entity;
using Security = AzurePlayground.Domain.Security;

namespace AzurePlayground.Database {
    public interface IPlaygroundContext : IDisposable {
        IDbSet<Security.User> Users { get; set; }
        IDbSet<Security.UserEventType> UserEventTypes { get; set; }
        int SaveChanges();
    }
}