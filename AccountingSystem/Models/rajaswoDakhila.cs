using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class rajaswoDakhila
    {
        [Key]
        public int rdId { get; set; }
        [Required]
        public string jornalNo { get; set; }
        public decimal kha14151 { get; set; }
        public decimal kha14213 { get; set; }
        public decimal kha14223 { get; set; }
        public decimal kha14227 { get; set; }
        public decimal kha15112 { get; set; }
        public decimal kha14212 { get; set; }
        public DateTime nepDate { get; set; }
        public string year { get; set; }
        public int monthIndex { get; set; }
        public string month { get; set; }
        public string nepFy { get; set; }
        public string nepDateStr { get; set; }
        public int fyId { get; set; }
        public virtual fiscalYear fiscalYear { get; set; }

    }
}