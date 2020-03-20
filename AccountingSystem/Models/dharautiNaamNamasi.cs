using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class dharautiNaamNamasi
    {
        [Key]
        public int dnId { get; set; }
        public int vendorId { get; set; }
        public virtual vendor vendor { get; set; }
        [Required(ErrorMessage = "Bibaran is Required")]
        public string bibaran { get; set; }
        [Required(ErrorMessage = "Rakam is Required")]
        public decimal rakam { get; set; }
        [Required(ErrorMessage = "Submission Date is Required")]
        public DateTime subDate { get; set; }
        public string nepFy { get; set; }
        public string month { get; set; }
        [Required(ErrorMessage = "Upto Date is is Required")]
        public DateTime uptoDate { get; set; }
        public int monthIndex { get; set; }
    }
}