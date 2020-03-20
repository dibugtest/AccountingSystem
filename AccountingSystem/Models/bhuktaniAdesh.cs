using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class bhuktaniAdesh
    {
        public int bhuktaniAdeshId { get; set; }
        [Required]
        public string bhuktaniAdeshNo { get; set; }
        public int baUSiNaId { get; set; }
        public virtual baUSiNa baUSiNa { get; set; }
        //public int khaSiNaId { get; set; }
        //public virtual khaSiNa khaSiNa { get; set; }
        public string month { get; set; }
        public DateTime nepDate { get; set; }
        [Required]
        public decimal rakam { get; set; }
        [Required]
        public string pauneKoNaam { get; set; }
        public string pauneKoCode { get; set; }
         [Required]
        public string bibaran { get; set; }
        public string khaSiNo { get; set; }
        public string reamrks { get; set; }
        public string jornalKharchaNo { get; set; }
        public string jornalNikasiNo { get; set; }
        public string year { get; set; }
        public string bhuktaniType { get; set; }
        public int monthIndex { get; set; }
        public string panNo { get; set; }
        public string BhuPraNo { get; set; }
        public string nepDateStr { get; set; }
        public DateTime enDate { get; set; }
        public int fyId { get; set; }
        public virtual fiscalYear fiscalYear { get; set; }
    }
}