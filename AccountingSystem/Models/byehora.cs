using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class byehora
    {
        [Key]
        public int byehoraId { get; set; }
        [Required]
        public string byehoraName { get; set; }
        public string hisabNo { get; set; }
        public int? khaSiNaId { get; set; }
        public virtual khaSiNa khaSiNa { get; set; }
        public string remarks { get; set; }
    }
}