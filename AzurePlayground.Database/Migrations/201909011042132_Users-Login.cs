namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersLogin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Security.UserEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        UserEventType = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Security.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            AddColumn("Security.Users", "PasswordSalt", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordHash", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordHashIterations", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("Security.UserEvents", "User_Id", "Security.Users");
            DropIndex("Security.UserEvents", new[] { "User_Id" });
            DropColumn("Security.Users", "PasswordHashIterations");
            DropColumn("Security.Users", "PasswordHash");
            DropColumn("Security.Users", "PasswordSalt");
            DropTable("Security.UserEvents");
        }
    }
}
