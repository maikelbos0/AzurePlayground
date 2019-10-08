namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordResetTokenFieldNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "PasswordResetToken_Salt", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordResetToken_Hash", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordResetToken_HashIterations", c => c.Int());
            AddColumn("Security.Users", "PasswordResetToken_ExpiryDate", c => c.DateTime());
            DropColumn("Security.Users", "PasswordResetTokenSalt");
            DropColumn("Security.Users", "PasswordResetTokenHash");
            DropColumn("Security.Users", "PasswordResetTokenHashIterations");
            DropColumn("Security.Users", "PasswordResetTokenExpiryDate");
        }
        
        public override void Down()
        {
            AddColumn("Security.Users", "PasswordResetTokenExpiryDate", c => c.DateTime());
            AddColumn("Security.Users", "PasswordResetTokenHashIterations", c => c.Int());
            AddColumn("Security.Users", "PasswordResetTokenHash", c => c.Binary(maxLength: 20));
            AddColumn("Security.Users", "PasswordResetTokenSalt", c => c.Binary(maxLength: 20));
            DropColumn("Security.Users", "PasswordResetToken_ExpiryDate");
            DropColumn("Security.Users", "PasswordResetToken_HashIterations");
            DropColumn("Security.Users", "PasswordResetToken_Hash");
            DropColumn("Security.Users", "PasswordResetToken_Salt");
        }
    }
}
