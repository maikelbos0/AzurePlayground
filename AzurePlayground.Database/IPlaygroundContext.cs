using System;
using System.Data.Entity;
using Security = AzurePlayground.Domain.Security;
using Auditing = AzurePlayground.Domain.Auditing;

namespace AzurePlayground.Database {
    public interface IPlaygroundContext : IDisposable {
        IDbSet<Security.User> Users { get; set; }
        IDbSet<Auditing.CommandExecution> CommandExecutions { get; set; }
        int SaveChanges();
    }
}