namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditingQueryExecutions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Auditing.QueryExecutions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        QueryType = c.String(nullable: false, maxLength: 255),
                        QueryData = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Auditing.QueryExecutions");
        }
    }
}
