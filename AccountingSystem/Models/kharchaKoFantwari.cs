using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class kharchaKoFantwari
    {
        public string khaSiNo { get; set; }
        public string khaSirsak { get; set; }
        public decimal kharchaThisMonth { get; set; }
        public decimal kharchaTillThisMonth { get; set; }
        public decimal budgetThisYear { get; set; }
        public decimal remainingBudget { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string baUSiNo { get; set; }
        public string nepFy { get; set; }
    }
}