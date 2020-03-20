namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dharautiAmdanis",
                c => new
                    {
                        dharautiAmdaniId = c.Int(nullable: false, identity: true),
                        nepDate = c.String(),
                        dharautiRakhne = c.String(),
                        billNo = c.String(),
                        fyId = c.Int(nullable: false),
                        monthIndex = c.Int(nullable: false),
                        billRakamNoVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatRakam = c.Decimal(nullable: false, precision: 18, scale: 2),
                        dharautiRakam = c.Decimal(nullable: false, precision: 18, scale: 2),
                        bapat = c.String(),
                        jornalNo = c.String(),
                    })
                .PrimaryKey(t => t.dharautiAmdaniId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.dharautiAmdanis");
        }
    }
}
