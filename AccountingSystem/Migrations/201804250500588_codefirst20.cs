namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ayaKars", "bakiPaune", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ayaKars", "bakiPaune");
        }
    }
}
