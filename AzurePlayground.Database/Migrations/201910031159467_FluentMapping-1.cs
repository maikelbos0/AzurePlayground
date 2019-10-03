namespace AzurePlayground.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FluentMapping1 : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "Security.Users", name: "uq_Email", newName: "IX_Email");
            AlterColumn("Security.Users", "PasswordSalt", c => c.Binary(nullable: false, maxLength: 20));
            AlterColumn("Security.Users", "PasswordHash", c => c.Binary(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("Security.Users", "PasswordHash", c => c.Binary(maxLength: 20));
            AlterColumn("Security.Users", "PasswordSalt", c => c.Binary(maxLength: 20));
            RenameIndex(table: "Security.Users", name: "IX_Email", newName: "uq_Email");
        }
    }
}
