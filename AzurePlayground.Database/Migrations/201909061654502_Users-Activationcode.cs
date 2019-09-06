namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersActivationcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("Security.Users", "ActivationCode", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("Security.Users", "ActivationCode");
        }
    }
}
