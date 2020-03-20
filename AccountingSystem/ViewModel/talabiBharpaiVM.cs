using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountingSystem.Models;

namespace AccountingSystem.ViewModel
{
    public class talabiBharpaiVM
    {

         //T.talabiId,T.suruScaleGrade,T.kaSaKoThap,T.suruBimaTotal,T.talabiBhattaTotal,T.kaSaKoKatti,T.sapati,T.naLaKos,T.pariKar,T.saSuKar,T.kattiTotal,T.pauneTotal,T.yearMonthId,T.officerId
         //                           ,O.fullName,O.pisNo,O.darja,O.bankAccNo,O.sthaiLekhaNo,O.bimaPariNo,O.bimaCode,O.bimaSheetRollNo
         //                           ,O.suruScale,O.gradeDar,O.gradeSankhya,O.mahangiBhatta,O.jokhimBhatta,O.bima,O.status,O.jobType,Y.year,Y.month

        public int officerId { get; set; }
        public int yearMonthId { get; set; }
        public int talabiId { get; set; }
        //सुरु स्केल+ग्रेड
        public decimal  suruScaleGrade  { get; set; }
        //10% of suruScaleGrade
        public decimal kaSaKoThap { get; set; }
        //suruScale_grade+KaSaKoThap+bima
        public decimal suruBimaTotal { get; set; }
        public decimal talabiBhattaTotal { get; set; }
        //20% of suruScaleGrade
        public decimal kaSaKoKatti { get; set; }
        public decimal sapati { get; set; }
        public decimal naLaKos { get; set; }
        public decimal pariKar { get; set; }
        public decimal saSuKar { get; set; }
        //kaSaKoKatti+sapati+naLaKos+pariKar+saSuKar
        public decimal kattiTotal { get; set; }
        //talabiBhattaTotal-kattiTotal
        public decimal pauneTotal { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string date { get; set; }
        public int  monthIndex { get; set; }
        public int fyId { get; set; }
      
         public string fullName { get; set; }
        public string pisNo { get; set; }
       
        public string email { get; set; }
        public string officePhone { get; set; }
       
        public string extenNo { get; set; }
       
        public string mobileNo { get; set; }
       
        public string darja { get; set; }
        //Bank
        public string bankAccNo { get; set; }
        //Bank कैफियत
        public string bankShakha { get; set; }
        //Tax
        public string sthaiLekhaNo { get; set; }
        //Bima
        public string bimaPariNo { get; set; }
        public string bimaCode { get; set; }
        public string bimaSheetRollNo { get; set; }
       
        //Talabi Bharpai
        public decimal suruScale { get; set; }
        public decimal mahangiBhatta { get; set; }
        public decimal jokhimBhatta { get; set; }
        public decimal bima { get; set; }
        public decimal gradeDar { get; set; }
        public decimal gradeSankhya { get; set; }
        public decimal saSukar { get; set; }
        public decimal paKar { get; set; }
        public string naLaKosNo { get; set; }
        public string kaSaKosNo { get; set; }
        public decimal kaSaKos { get; set; }
        public string status { get; set; }
        public string jobType { get; set; }

        //public List<talabiBharpai> talabiBharpais { get; set; }
    }
}