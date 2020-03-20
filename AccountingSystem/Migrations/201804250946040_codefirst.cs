namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ayaKars", "hisabNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ayaKars", "hisabNo");
        }
    }
}
