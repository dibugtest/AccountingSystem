using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class hisab
    {
        public string baUSiNo { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public DateTime nepDate { get; set; }
        public string jornalNo { get; set; }
        public string bibaran { get; set; }
        public decimal talab { get; set; }
        public decimal mahangiBhatta { get; set; }
        public decimal anyaBhatta { get; set; }
        public decimal posak { get; set; }
        public decimal paniBijuli { get; set; }
        public decimal sancharMahasul { get; set; }
        public decimal endhan { get; set; }
        public decimal sanchalanMarmat { get; set; }
        public decimal bima { get; set; }
        public decimal karyalayaSambandhi { get; set; }
        public decimal pustakSamagri { get; set; }
        public decimal sewaParamarsa { get; set; }
        public decimal anyaSewaSulka { get; set; }
        public decimal karyakram { get; set; }
        public decimal bibidhaKaryakram { get; set; }
        public decimal anugamanMulyankan { get; set; }
        public decimal bhraman { get; set; }
        public decimal bibidha { get; set; }
        public decimal furniture { get; set; }
        public decimal machineri { get; set; }
        public decimal  pujigatSudhar { get; set; }
        public string bajetHisabType { get; set; }
        public string nepFy { get; set; }
        public string nepDateStr { get; set; }
        public int fyId { get; set; }
    }
}