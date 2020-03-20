using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class bhuktaniAdeshTalabi
    {
        public string bhuktaniAdeshNo { get; set; }
        public string month { get; set; }
        public DateTime nepDate { get; set; }
        public decimal rakam { get; set; }
        public string rakamInWords { get; set; }
        public string pauneKoNaam { get; set; }
        public string pauneKoCode { get; set; }
        public string bibaran { get; set; }
        public string khaSiNo { get; set; }
        public string reamrks { get; set; }
        public string year { get; set; }
        public string nepFy { get; set; }
        public string jornalNo { get; set; }
        public string panNo { get; set; }
        public string BhuPraNo { get; set; }
        public string  nepDateStr { get; set; }
       
    }
}