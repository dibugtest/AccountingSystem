using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public enum month
    {
        श्रावण, भाद्र, अशोज, कार्तिक, मङ्सिर, पुष, माघ, फाल्गुन, चैत्र, वैशाख, जेठ, असार
    }
    public class yearMonth
    {   [Key]
        public int yearMonthId { get; set; }
        [Required]
        public string year { get; set; }
        [Required]
        public string month { get; set; }
        public string date { get; set; }
        public int  monthIndex { get; set; }
        public int fyId { get; set; }
        public virtual fiscalYear  fiscalYear { get; set; } 
        public ICollection<talabiBharpai> talabiBharpais { get; set; }
    }
}