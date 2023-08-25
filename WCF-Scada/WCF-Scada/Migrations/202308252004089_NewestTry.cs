namespace WCF_Scada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewestTry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alarms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        ThresholdValue = c.Double(nullable: false),
                        VariableName = c.String(),
                        AnalogInputTagId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalogInputTags", t => t.AnalogInputTagId)
                .Index(t => t.AnalogInputTagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alarms", "AnalogInputTagId", "dbo.AnalogInputTags");
            DropIndex("dbo.Alarms", new[] { "AnalogInputTagId" });
            DropTable("dbo.Alarms");
        }
    }
}
