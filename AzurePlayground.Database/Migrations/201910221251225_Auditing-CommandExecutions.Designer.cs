// <auto-generated />
namespace AzurePlayground.Database.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.2.0-61023")]
    public sealed partial class AuditingCommandExecutions : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AuditingCommandExecutions));
        
        string IMigrationMetadata.Id
        {
            get { return "201910221251225_Auditing-CommandExecutions"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
