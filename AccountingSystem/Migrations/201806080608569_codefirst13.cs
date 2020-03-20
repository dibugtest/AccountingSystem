namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.bhuktaniAdeshes", "nepDateStr", c => c.String());
            AddColumn("dbo.bhuktaniAdeshes", "enDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.bhuktaniAdeshes", "enDate");
            DropColumn("dbo.bhuktaniAdeshes", "nepDateStr");
        }
    }
}
