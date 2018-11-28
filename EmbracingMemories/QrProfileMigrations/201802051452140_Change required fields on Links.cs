namespace EmbracingMemories.QrProfileMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangerequiredfieldsonLinks : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QrLink", "Label", c => c.String(nullable: false));
            AlterColumn("dbo.QrLink", "Url", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QrLink", "Url", c => c.String());
            AlterColumn("dbo.QrLink", "Label", c => c.String());
        }
    }
}
