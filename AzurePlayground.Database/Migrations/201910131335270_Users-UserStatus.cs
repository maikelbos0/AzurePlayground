namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersUserStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Security.UserStatus",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("Security.Users", "UserStatus_Id", c => c.Byte(nullable: false));
            CreateIndex("Security.Users", "UserStatus_Id");
            AddForeignKey("Security.Users", "UserStatus_Id", "Security.UserStatus", "Id", cascadeDelete: true);
            DropColumn("Security.Users", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("Security.Users", "IsActive", c => c.Boolean(nullable: false));
            DropForeignKey("Security.Users", "UserStatus_Id", "Security.UserStatus");
            DropIndex("Security.Users", new[] { "UserStatus_Id" });
            DropColumn("Security.Users", "UserStatus_Id");
            DropTable("Security.UserStatus");
        }
    }
}
