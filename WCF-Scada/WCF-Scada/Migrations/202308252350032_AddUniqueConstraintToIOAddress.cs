namespace WCF_Scada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueConstraintToIOAddress : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tags", "IOAddress", c => c.String(maxLength: 255));
            CreateIndex("dbo.Tags", "IOAddress", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tags", new[] { "IOAddress" });
            AlterColumn("dbo.Tags", "IOAddress", c => c.String());
        }
    }
}
