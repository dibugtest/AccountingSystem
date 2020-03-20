using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public enum month
    { 
        वैशाख,जेठ,असार,श्रावण,भाद्र,अशोज,कार्तिक,मङ्सिर,पुष,माघ,फाल्गुन,चैत्र
    }
    public class payroll
    {
        public int payrollId { get; set; }
        public string Name { get; set; }
        public status status { get; set; }
        public decimal bhatta { get; set; }
        public decimal talab { get; set; }
        public decimal bima { get; set; }
        public decimal vat { get; set; }
        public decimal deduction { get; set; }
        public decimal grossTotal { get; set; }
        public string remarks { get; set; }
        public month month { get; set; }
        public string year { get; set; }

    }
}