namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersPasswordReset : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "PasswordResetTokenHash", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordResetTokenExpiryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Security.Users", "PasswordResetTokenExpiryDate");
            DropColumn("Security.Users", "PasswordResetTokenHash");
        }
    }
}
