namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.yearMonths", "fyId", c => c.Int(nullable: false));
            CreateIndex("dbo.yearMonths", "fyId");
            AddForeignKey("dbo.yearMonths", "fyId", "dbo.fiscalYears", "fyId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.yearMonths", "fyId", "dbo.fiscalYears");
            DropIndex("dbo.yearMonths", new[] { "fyId" });
            DropColumn("dbo.yearMonths", "fyId");
        }
    }
}
