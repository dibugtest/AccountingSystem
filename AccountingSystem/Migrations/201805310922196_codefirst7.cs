namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst7 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.dharautiAmdanis", "fyId");
            AddForeignKey("dbo.dharautiAmdanis", "fyId", "dbo.fiscalYears", "fyId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dharautiAmdanis", "fyId", "dbo.fiscalYears");
            DropIndex("dbo.dharautiAmdanis", new[] { "fyId" });
        }
    }
}
