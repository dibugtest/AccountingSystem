using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class bhuktaniPaune
    {
        [Key]
        public int bhuktaniPauneId { get; set; }
        [Required]
        public string pauneKoNaam { get; set; }
        public string address { get; set; }
        public string code { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string remarks { get; set; }
        public string pauneType { get; set; }
    }
}