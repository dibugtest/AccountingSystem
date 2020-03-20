using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class fiscalYear
    {[Key]
        public int fyId { get; set; }
        [Required(ErrorMessage="Nepali Fiscal year is Required")]
        public string nepFy { get; set; }
        [Required(ErrorMessage = "English Fiscal year is Required")]
        public string enFy { get; set; }
        [Required]
        public int nepStartYear { get; set; }
        [Required]
        public int nepEndYear { get; set; }
        [Required]
        public int enStartYear { get; set; }
        [Required]
        public int enEndYear { get; set; }
        public Boolean status { get; set; }
        public DateTime activatedDate { get; set; }
        public string activatedBy { get; set; }
    }
}