namespace EmbracingMemories.QrProfileMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQrProfileCreateTimestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QrProfile", "CreateTimestamp", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.QrProfile", "CreateTimestamp");
        }
    }
}
