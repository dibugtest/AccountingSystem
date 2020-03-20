using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class ayaKar
    {
        [Key]
        public int ayaKarId { get; set; }
        public string  firmName { get; set; }
        public decimal aayaKar { get; set; }
        public decimal paKar { get; set; }
        public decimal dharauti { get; set; }
        public decimal jammaRakam { get; set; }
        public decimal bakiPaune { get; set; }
        public string jornalKattiNo { get; set; }
        public string jornalKharchaNo { get; set; }
        public string hisabNo { get; set; }
        public int fyId { get; set; }
        public int monthIndex { get; set; }
        public int baUSiNaId { get; set; }
    }
}