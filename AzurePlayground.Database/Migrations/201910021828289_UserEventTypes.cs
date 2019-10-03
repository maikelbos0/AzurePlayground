namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEventTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Security.UserEventTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("Security.UserEvents", "UserEventType_Id", c => c.Byte(nullable: false));
            CreateIndex("Security.UserEvents", "UserEventType_Id");
            AddForeignKey("Security.UserEvents", "UserEventType_Id", "Security.UserEventTypes", "Id", cascadeDelete: true);
            DropColumn("Security.UserEvents", "UserEventType");
        }
        
        public override void Down()
        {
            AddColumn("Security.UserEvents", "UserEventType", c => c.Byte(nullable: false));
            DropForeignKey("Security.UserEvents", "UserEventType_Id", "Security.UserEventTypes");
            DropIndex("Security.UserEvents", new[] { "UserEventType_Id" });
            DropColumn("Security.UserEvents", "UserEventType_Id");
            DropTable("Security.UserEventTypes");
        }
    }
}
