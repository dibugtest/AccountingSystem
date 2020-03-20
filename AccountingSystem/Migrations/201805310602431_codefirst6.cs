namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ayaKars", "baUSiNaId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ayaKars", "baUSiNaId");
        }
    }
}
