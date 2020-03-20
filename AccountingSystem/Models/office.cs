using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class office
    {
        [Key]
        public int officeId { get; set; }
        public string mantralaye { get; set; }
        public string bibhag { get; set; }
        [Required]
        public string karyalaye { get; set; }
        [Required]
        public string address { get; set; }
        public string phone { get; set; }
        public string url_web { get; set; }
        public string email { get; set; }
        public string officeCode { get; set; }
        public string POBoxNo { get; set; }
    }
}