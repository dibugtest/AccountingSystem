namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.fiscalYears", "nepStartYear", c => c.Int(nullable: false));
            AddColumn("dbo.fiscalYears", "nepEndYear", c => c.Int(nullable: false));
            AddColumn("dbo.fiscalYears", "enStartYear", c => c.Int(nullable: false));
            AddColumn("dbo.fiscalYears", "enEndYear", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.fiscalYears", "enEndYear");
            DropColumn("dbo.fiscalYears", "enStartYear");
            DropColumn("dbo.fiscalYears", "nepEndYear");
            DropColumn("dbo.fiscalYears", "nepStartYear");
        }
    }
}
