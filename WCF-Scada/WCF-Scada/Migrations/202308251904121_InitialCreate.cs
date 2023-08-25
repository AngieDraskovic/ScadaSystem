namespace WCF_Scada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        IOAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InputTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DriverType = c.Int(nullable: false),
                        ScanTime = c.Int(nullable: false),
                        OnScan = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AnalogInputTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LowLimit = c.Double(nullable: false),
                        HighLimit = c.Double(nullable: false),
                        Units = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InputTags", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.OutputTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        InitialValue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AnalogOutputTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LowLimit = c.Double(nullable: false),
                        HighLimit = c.Double(nullable: false),
                        Units = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OutputTags", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DigitalInputTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InputTags", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DigitalOutputTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OutputTags", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DigitalOutputTags", "Id", "dbo.OutputTags");
            DropForeignKey("dbo.DigitalInputTags", "Id", "dbo.InputTags");
            DropForeignKey("dbo.AnalogOutputTags", "Id", "dbo.OutputTags");
            DropForeignKey("dbo.OutputTags", "Id", "dbo.Tags");
            DropForeignKey("dbo.AnalogInputTags", "Id", "dbo.InputTags");
            DropForeignKey("dbo.InputTags", "Id", "dbo.Tags");
            DropIndex("dbo.DigitalOutputTags", new[] { "Id" });
            DropIndex("dbo.DigitalInputTags", new[] { "Id" });
            DropIndex("dbo.AnalogOutputTags", new[] { "Id" });
            DropIndex("dbo.OutputTags", new[] { "Id" });
            DropIndex("dbo.AnalogInputTags", new[] { "Id" });
            DropIndex("dbo.InputTags", new[] { "Id" });
            DropTable("dbo.DigitalOutputTags");
            DropTable("dbo.DigitalInputTags");
            DropTable("dbo.AnalogOutputTags");
            DropTable("dbo.OutputTags");
            DropTable("dbo.AnalogInputTags");
            DropTable("dbo.InputTags");
            DropTable("dbo.Tags");
        }
    }
}
