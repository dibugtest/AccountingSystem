using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class rajaswoSirsak
    {
        [Key]
        public int rsId { get; set; }
        [Required]
        public string sirsak { get; set; }
        [Required]
        public string sirsakNo { get; set; }
        public string  remarks { get; set; }
    }
}