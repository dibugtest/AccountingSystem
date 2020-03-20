namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.rajaswoAsulis", "nepDateStr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.rajaswoAsulis", "nepDateStr");
        }
    }
}
