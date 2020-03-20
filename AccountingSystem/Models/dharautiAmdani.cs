using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class dharautiAmdani
    {
        public int dharautiAmdaniId { get; set; }
        public string nepDate { get; set; }
        public string dharautiRakhne { get; set; }
        public string billNo { get; set; }
        public int fyId { get; set; }
        public int monthIndex { get; set; }
        public decimal billRakamNoVat { get; set; }
        public decimal VatRakam { get; set; }
        public decimal dharautiRakam { get; set; }
        public string bapat { get; set; }
        public string jornalNo { get; set; }
        public virtual fiscalYear fiscalYear { get; set; }
       
    }
}