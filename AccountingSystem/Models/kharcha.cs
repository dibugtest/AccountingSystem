using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class kharcha
    {
        [Key]
        public int kharchaId { get; set; }
        public decimal  expThisMonth { get; set; }
        public decimal expTillThisMonth { get; set; }
        public string khaPaNo { get; set; }
        public string khaSirsak { get; set; }
        public decimal yearlyBudget { get; set; }
        public decimal remainBudget{get; set; }
        public DateTime nepDate { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public int baUSiNaId { get; set; }
        public virtual baUSiNa baUSiNa { get; set; }
        public string nepFy { get; set; }
    }
}