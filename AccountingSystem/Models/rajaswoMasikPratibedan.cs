using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class rajaswoMasikPratibedan
    {
        public string rajaswoSirsak { get; set; }
        public string amdaniBargikaran { get; set; }
        public decimal uptoPrevMonth { get; set; }
        public decimal totalThisMonth { get; set; }
        public decimal totalUptoThisMonth { get; set; }
        public string remarks { get; set; }
        public decimal nagadMaujdat { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public string nepFy { get; set; }
        public decimal prevNagadiMaujdat { get; set; }
        public decimal totalBankNagadi { get; set; }
    }
}