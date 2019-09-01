namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersRemoveName : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Security.Users", "Email", unique: true, name: "uq_Email");
            DropColumn("Security.Users", "UserName");
        }
        
        public override void Down()
        {
            AddColumn("Security.Users", "UserName", c => c.String(nullable: false, maxLength: 255));
            DropIndex("Security.Users", "uq_Email");
        }
    }
}
