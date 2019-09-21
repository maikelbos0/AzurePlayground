namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersPasswordReset : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "PasswordResetTokenSalt", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordResetTokenHash", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordResetTokenHashIterations", c => c.Int());
            AddColumn("Security.Users", "PasswordResetTokenExpiryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Security.Users", "PasswordResetTokenExpiryDate");
            DropColumn("Security.Users", "PasswordResetTokenHashIterations");
            DropColumn("Security.Users", "PasswordResetTokenHash");
            DropColumn("Security.Users", "PasswordResetTokenSalt");
        }
    }
}
