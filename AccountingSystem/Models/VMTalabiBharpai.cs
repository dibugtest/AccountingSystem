using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class VMTalabiBharpai
    {
        public string fullName { get; set; }
        public string darja { get; set; }
        public int talabiId { get; set; }
        //सुरु स्केल+ग्रेड
        public decimal suruScaleGrade { get; set; }
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
        public int officerId { get; set; }
        public int yearMonthId { get; set; }
        public string nepFy { get; set; }
        public int fyId { get; set; }
        //Added Items 
        public decimal suruScale { get; set; }
        public decimal gradeRakam { get; set; }
        public decimal bima { get; set; }
        public decimal mahangiBhatta { get; set; }
        public decimal jokhimBhatta { get; set; }
    }
}