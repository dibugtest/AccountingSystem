using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class budget
    {
        public int budgetId { get; set; }
        [Required]
        public string khaSiNo { get; set; }
        [Required]
        public decimal budgetAmount { get; set; }
        [Required]
        public string nepFy { get; set; }
        [Required(ErrorMessage = "English Fiscal year is Required")]
        public string enFy { get; set; }
        public string remarks { get; set; }
    }
}