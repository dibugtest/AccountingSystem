namespace AccountingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class codefirst11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.officers", "paKar", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterStoredProcedure(
                "dbo.officer_Insert",
                p => new
                    {
                        fullName = p.String(),
                        pisNo = p.String(),
                        email = p.String(),
                        officePhone = p.String(),
                        extenNo = p.String(),
                        mobileNo = p.String(),
                        darja = p.String(),
                        bankAccNo = p.String(),
                        bankShakha = p.String(),
                        sthaiLekhaNo = p.String(),
                        bimaPariNo = p.String(),
                        bimaCode = p.String(),
                        bimaSheetRollNo = p.String(),
                        suruScale = p.Decimal(precision: 18, scale: 2),
                        mahangiBhatta = p.Decimal(precision: 18, scale: 2),
                        jokhimBhatta = p.Decimal(precision: 18, scale: 2),
                        bima = p.Decimal(precision: 18, scale: 2),
                        gradeDar = p.Decimal(precision: 18, scale: 2),
                        gradeSankhya = p.Decimal(precision: 18, scale: 2),
                        saSukar = p.Decimal(precision: 18, scale: 2),
                        paKar = p.Decimal(precision: 18, scale: 2),
                        naLaKosNo = p.String(),
                        naLaKos = p.Decimal(precision: 18, scale: 2),
                        kaSaKosNo = p.String(),
                        kaSaKos = p.Decimal(precision: 18, scale: 2),
                        status = p.String(),
                        jobType = p.String(),
                        officeId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[officers]([fullName], [pisNo], [email], [officePhone], [extenNo], [mobileNo], [darja], [bankAccNo], [bankShakha], [sthaiLekhaNo], [bimaPariNo], [bimaCode], [bimaSheetRollNo], [suruScale], [mahangiBhatta], [jokhimBhatta], [bima], [gradeDar], [gradeSankhya], [saSukar], [paKar], [naLaKosNo], [naLaKos], [kaSaKosNo], [kaSaKos], [status], [jobType], [officeId])
                      VALUES (@fullName, @pisNo, @email, @officePhone, @extenNo, @mobileNo, @darja, @bankAccNo, @bankShakha, @sthaiLekhaNo, @bimaPariNo, @bimaCode, @bimaSheetRollNo, @suruScale, @mahangiBhatta, @jokhimBhatta, @bima, @gradeDar, @gradeSankhya, @saSukar, @paKar, @naLaKosNo, @naLaKos, @kaSaKosNo, @kaSaKos, @status, @jobType, @officeId)
                      
                      DECLARE @officerId int
                      SELECT @officerId = [officerId]
                      FROM [dbo].[officers]
                      WHERE @@ROWCOUNT > 0 AND [officerId] = scope_identity()
                      
                      SELECT t0.[officerId]
                      FROM [dbo].[officers] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[officerId] = @officerId"
            );
            
            AlterStoredProcedure(
                "dbo.officer_Update",
                p => new
                    {
                        officerId = p.Int(),
                        fullName = p.String(),
                        pisNo = p.String(),
                        email = p.String(),
                        officePhone = p.String(),
                        extenNo = p.String(),
                        mobileNo = p.String(),
                        darja = p.String(),
                        bankAccNo = p.String(),
                        bankShakha = p.String(),
                        sthaiLekhaNo = p.String(),
                        bimaPariNo = p.String(),
                        bimaCode = p.String(),
                        bimaSheetRollNo = p.String(),
                        suruScale = p.Decimal(precision: 18, scale: 2),
                        mahangiBhatta = p.Decimal(precision: 18, scale: 2),
                        jokhimBhatta = p.Decimal(precision: 18, scale: 2),
                        bima = p.Decimal(precision: 18, scale: 2),
                        gradeDar = p.Decimal(precision: 18, scale: 2),
                        gradeSankhya = p.Decimal(precision: 18, scale: 2),
                        saSukar = p.Decimal(precision: 18, scale: 2),
                        paKar = p.Decimal(precision: 18, scale: 2),
                        naLaKosNo = p.String(),
                        naLaKos = p.Decimal(precision: 18, scale: 2),
                        kaSaKosNo = p.String(),
                        kaSaKos = p.Decimal(precision: 18, scale: 2),
                        status = p.String(),
                        jobType = p.String(),
                        officeId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[officers]
                      SET [fullName] = @fullName, [pisNo] = @pisNo, [email] = @email, [officePhone] = @officePhone, [extenNo] = @extenNo, [mobileNo] = @mobileNo, [darja] = @darja, [bankAccNo] = @bankAccNo, [bankShakha] = @bankShakha, [sthaiLekhaNo] = @sthaiLekhaNo, [bimaPariNo] = @bimaPariNo, [bimaCode] = @bimaCode, [bimaSheetRollNo] = @bimaSheetRollNo, [suruScale] = @suruScale, [mahangiBhatta] = @mahangiBhatta, [jokhimBhatta] = @jokhimBhatta, [bima] = @bima, [gradeDar] = @gradeDar, [gradeSankhya] = @gradeSankhya, [saSukar] = @saSukar, [paKar] = @paKar, [naLaKosNo] = @naLaKosNo, [naLaKos] = @naLaKos, [kaSaKosNo] = @kaSaKosNo, [kaSaKos] = @kaSaKos, [status] = @status, [jobType] = @jobType, [officeId] = @officeId
                      WHERE ([officerId] = @officerId)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.officers", "paKar");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
