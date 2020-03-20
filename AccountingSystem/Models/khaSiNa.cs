using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class khaSiNa
    {
        [Key]
        public int  khaSiNaId { get; set; }
        [Required]
        public string khaSirsak { get; set; }
        [Required]
        public string khaSiNo { get; set; }
        public decimal khaRakam { get; set; }
        public string remarks { get; set; }
        [Required]
        public int baUSiNaId { get; set; }
        public virtual baUSiNa baUSiNa { get; set; }

    }
}