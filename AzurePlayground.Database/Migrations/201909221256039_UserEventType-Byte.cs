namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEventTypeByte : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Security.UserEvents", "UserEventType", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Security.UserEvents", "UserEventType", c => c.Int(nullable: false));
        }
    }
}
