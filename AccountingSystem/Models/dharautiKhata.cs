using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class dharautiKhata
    {
        public string bibaran { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public string nepFy { get; set; }
        public string nepDate { get; set; }
        public decimal debit { get; set; }
        public decimal credit { get; set; }
        public decimal baki { get; set; }
        public string jornalNo { get; set; }
        public int serialNo { get; set; }
        public string kaifiyat { get; set; }
    }
}