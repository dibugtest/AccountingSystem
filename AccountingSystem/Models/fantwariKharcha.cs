using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class fantwariKharcha
    {
        [Key]
        public int fantwariId { get; set; }
        public decimal kharchaThisMonth { get; set; }
        public decimal kharchaTillMonth { get; set; }
        public string khaSiNo { get; set; }
        public string khaSirsak { get; set; }
        public decimal yearlyBudget { get; set; }
        public decimal remainingBudget { get; set; }
        public int baUSiNaId { get; set; }
        public virtual baUSiNa baUSiNa { get; set; }
        public string  year { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public string nepFy { get; set; }
        public DateTime nepDate { get; set; }

    }
}