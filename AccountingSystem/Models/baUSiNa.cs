using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class baUSiNa
    {
        [Key]
        public int baUSiNaId { get; set; }
        [Required]
        public string baUSirsak { get; set; }
        public string baUSiNo { get; set; }
        public string remarks { get; set; }
        public ICollection<khaSiNa> khaSiNas { get; set; }
    }
}