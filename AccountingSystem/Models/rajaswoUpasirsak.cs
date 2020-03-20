using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class rajaswoUpasirsak
    {
        [Key]
        public int ruId { get; set; }
        [Required]
        public string upaSirsak { get; set; }
        public int rsId { get; set; }
        public virtual rajaswoSirsak rajaswoSirsak { get; set; }
        public string remarks { get; set; }
    }
}