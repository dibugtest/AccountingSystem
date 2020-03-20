using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class nagadi
    {
        public DateTime nepDate { get; set; }
        public string jornalNo  { get; set; }
        public string bibaran { get; set; }
        public decimal tahabilDe { get; set; }
        public decimal tahabilCre { get; set; }
        public decimal bankDe { get; set; }
        public decimal bankCre { get; set; }
        public string  chequeNo { get; set; }
        public decimal maujdatBanki { get; set; }
        public string khaSiNo { get; set; }
        public decimal bajetKharcha { get; set; }
        public decimal peskiPayeko { get; set; }
        public decimal peskiFarkiyeko { get; set; }
        public string hisabNo { get; set; }
        public decimal bibidhaDe { get; set; }
        public decimal bibidhaCre { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public string  baUSiNo { get; set; }
        public string  nagadiType { get; set; }
        public string nepDateStr { get; set; }
        public int fyId { get; set; }
        public int serialNo { get; set; }
    }
}