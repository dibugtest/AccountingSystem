namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.jornalEntries", "nepDateStr", c => c.String());
            AddColumn("dbo.jornalEntries", "enDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.jornalEntries", "enDate");
            DropColumn("dbo.jornalEntries", "nepDateStr");
        }
    }
}
