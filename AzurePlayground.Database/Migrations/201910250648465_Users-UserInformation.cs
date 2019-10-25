namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersUserInformation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "DisplayName", c => c.String());
            AddColumn("Security.Users", "Description", c => c.String());
            AddColumn("Security.Users", "ShowEmail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Security.Users", "ShowEmail");
            DropColumn("Security.Users", "Description");
            DropColumn("Security.Users", "DisplayName");
        }
    }
}
