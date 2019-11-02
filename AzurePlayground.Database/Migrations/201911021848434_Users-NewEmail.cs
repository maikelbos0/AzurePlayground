namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersNewEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "NewEmail", c => c.String(maxLength: 255));
            AddColumn("Security.Users", "NewEmailConfirmationCode", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("Security.Users", "NewEmailConfirmationCode");
            DropColumn("Security.Users", "NewEmail");
        }
    }
}
