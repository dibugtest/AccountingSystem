namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.setNumbers", "baUSiNaId", c => c.Int(nullable: false));
            CreateIndex("dbo.setNumbers", "baUSiNaId");
            AddForeignKey("dbo.setNumbers", "baUSiNaId", "dbo.baUSiNas", "baUSiNaId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.setNumbers", "baUSiNaId", "dbo.baUSiNas");
            DropIndex("dbo.setNumbers", new[] { "baUSiNaId" });
            DropColumn("dbo.setNumbers", "baUSiNaId");
        }
    }
}
