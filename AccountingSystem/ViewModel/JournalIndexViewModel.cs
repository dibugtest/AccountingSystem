using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.ViewModel
{
    public class JournalIndexViewModel
    {
        [Range(1,6566,ErrorMessage="Must select one option from the list.")]
        public int baUSiNaId { get; set; }
        [Required(ErrorMessage="Must select one option from the list.")]
        public string type { get; set; }
        [Range(1, 6566, ErrorMessage = "Must select one option from the list.")]
        public int fyId { get; set; }
        [Range(0, 11, ErrorMessage = "Must select one option from the list.")]
        public int mn { get; set; }
        public IEnumerable<baUSiNa> baUSiNas { get; set; }
        public IEnumerable<fy> fy { get; set; }
        public IEnumerable<mnth> mnth { get; set; }
        public IEnumerable<typeName> typeName { get; set; }
    }
    public class baUSiNa {
        public int baUSiNaId { get; set; }
        public string baUSiNo { get; set; }
    }
    public class fy
    {
        public int fyId { get; set; }
        public string nepFy { get; set; }
    }
    public class mnth
    {
        public int id { get; set; }
        public string month { get; set; }
    }
public class typeName
{
   public string name { get; set; } 
}
}