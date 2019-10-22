namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditingCommandExecutions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Auditing.CommandExecutions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        CommandType = c.String(nullable: false, maxLength: 255),
                        CommandData = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Auditing.CommandExecutions");
        }
    }
}
