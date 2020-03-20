namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomePropertiesInTalabiBharpai : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.talabiBharpais", "suruScale", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.talabiBharpais", "gradeRakam", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.talabiBharpais", "bima", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.talabiBharpais", "mahangiBhatta", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.talabiBharpais", "jokhimBhatta", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.talabiBharpais", "jokhimBhatta");
            DropColumn("dbo.talabiBharpais", "mahangiBhatta");
            DropColumn("dbo.talabiBharpais", "bima");
            DropColumn("dbo.talabiBharpais", "gradeRakam");
            DropColumn("dbo.talabiBharpais", "suruScale");
        }
    }
}
