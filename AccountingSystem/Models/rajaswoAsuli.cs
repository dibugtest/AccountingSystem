using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class rajaswoAsuli
    {

        [Key]
        public int raId { get; set; }
        public DateTime nepDate { get; set; }
        public string rasidNo { get; set; }
        //[Required]
        public string bhujauneKoNaam { get; set; }
        public decimal kha14151 { get; set; }
        public decimal janmaMiti { get; set; }
        public decimal rePrint { get; set; }
        public decimal microFilm { get; set; }
        public decimal pratilipi { get; set; }
        public decimal lipyantar { get; set; }
        public decimal digitalImage { get; set; }
        public decimal kha14213 { get; set; }
        public decimal kha14227 { get; set; }
        public int monthIndex { get; set; }
        public string month { get; set; }
        public string nepFy { get; set; }
        public string year { get; set; }
        public string nepDateStr { get; set; }
        public int fyId { get; set; }
        public virtual fiscalYear fiscalYear { get; set; }
    }
}