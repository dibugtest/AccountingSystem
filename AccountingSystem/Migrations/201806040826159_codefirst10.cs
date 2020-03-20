namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.rajaswoDakhilas", "nepDateStr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.rajaswoDakhilas", "nepDateStr");
        }
    }
}
