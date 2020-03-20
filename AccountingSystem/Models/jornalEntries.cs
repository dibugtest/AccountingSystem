using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class jornalEntries
    {
        public int jornalEntriesId { get; set; }
        public string deCre { get; set; }
        public string jornalNo { get; set; }
        public int? baUSiNaId { get; set; }
        public virtual baUSiNa baUSiNa { get; set; }
        public string jornalType { get; set; }

        //public int yearMonthId { get; set; }
        //public virtual yearMonth yearMonth { get; set; }
        public string month { get; set; }
        public DateTime nepDate { get; set; }
        public string note { get; set; }
        public decimal debit { get; set; }
        //decrease assets and increase liablitity and equity,what goes out
        public decimal credit { get; set; }
        [Required]
        public string bibaran { get; set; }
        public string hisabNo { get; set; }
        public string khaPaNo { get; set; }
        public string sanketNo { get; set; }
        public string year { get; set; }
        public string  nepDateStr { get; set; }
        public DateTime enDate { get; set; }
        public int monthIndex { get; set; }
        public int fyId { get; set; }
        public virtual fiscalYear fiscalYear { get; set; }
        public decimal chequeRakam { get; set; }
    }

}