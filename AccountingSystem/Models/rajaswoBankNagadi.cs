using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class rajaswoBankNagadi
    {
        public string jornalNo { get; set; }
        public DateTime nepDate { get; set; }
        public string bibaran { get; set; }
        public decimal nagadiDe { get; set; }
        public decimal nagadiCre { get; set; }
        public decimal nagadiRemain { get; set; }
        public decimal bankDe { get; set; }
        public decimal bankCre { get; set; }
        public decimal bankRemain { get; set; }
        public decimal rajaswoDe { get; set; }
        public decimal rajswoCre { get; set; }
        public decimal rajswoRemain { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public string nepFy { get; set; }
    }
}