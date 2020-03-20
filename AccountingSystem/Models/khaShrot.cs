using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class khaShrot
    {
        [Key]
        public int sourceId { get; set; }
        [Required]
        public string sourceName { get; set; }
        public string remarks { get; set; }
    }
}