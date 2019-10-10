namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordFieldNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "Password_Salt", c => c.Binary(nullable: false, maxLength: 20));
            AddColumn("Security.Users", "Password_Hash", c => c.Binary(nullable: false, maxLength: 20));
            AddColumn("Security.Users", "Password_HashIterations", c => c.Int(nullable: false));
            DropColumn("Security.Users", "PasswordSalt");
            DropColumn("Security.Users", "PasswordHash");
            DropColumn("Security.Users", "PasswordHashIterations");
        }
        
        public override void Down()
        {
            AddColumn("Security.Users", "PasswordHashIterations", c => c.Int(nullable: false));
            AddColumn("Security.Users", "PasswordHash", c => c.Binary(nullable: false, maxLength: 20));
            AddColumn("Security.Users", "PasswordSalt", c => c.Binary(nullable: false, maxLength: 20));
            DropColumn("Security.Users", "Password_HashIterations");
            DropColumn("Security.Users", "Password_Hash");
            DropColumn("Security.Users", "Password_Salt");
        }
    }
}
