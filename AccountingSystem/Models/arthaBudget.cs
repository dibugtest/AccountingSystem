using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class arthaBudget
    {
        public int arthaBudgetId { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        [Required(ErrorMessage="Employee Name is Required")]
        public string employeeName { get; set; }
        //For KhaSiNa 27313,baUSiNa 6010143
        public decimal khaSiNa3 { get; set; }
        //For KhaSiNa 27314, baUSiNa 6010183
        public decimal khaSiNa4 { get; set; }
        public DateTime nepDate { get; set; }
        public int monthIndex { get; set; }
        public int fyId { get; set; }
        public virtual fiscalYear fiscalYear { get; set; }

    }
}