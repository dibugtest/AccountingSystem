using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public enum status
    {
      कार्यरत, पेन्सन, सरुवा, वेतलबी,अन्य
    }
    public enum jobType
    { 
        करार,स्थाई
        //permanent,temporary
    }
    
    public class officer
    {
        [Key]
        public int officerId { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string fullName { get; set; }
        [Required]
        [Display(Name = "PIS No.")]
        public string pisNo { get; set; }
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        public string officePhone { get; set; }
        public string extenNo { get; set; }
        [Display(Name = "Mobile No")]
        public string mobileNo { get; set; }
        [Required]
        public string darja { get; set; }
        //Bank
        public string bankAccNo { get; set; }
        //Bank कैफियत
        public string bankShakha { get; set; }
        //Tax
        public string sthaiLekhaNo { get; set; }
        //Bima
        public string bimaPariNo { get; set; }
        public string bimaCode { get; set; }
        public string bimaSheetRollNo { get; set; }
        [Required]
        //Talabi Bharpai
        public decimal suruScale { get; set; }
        public decimal mahangiBhatta { get; set; }
        public decimal jokhimBhatta { get; set; }
        public decimal bima { get; set; }
        public decimal gradeDar { get; set; }
        public decimal gradeSankhya { get; set; }
        public decimal saSukar { get; set; }
        public decimal paKar { get; set; }
        public string naLaKosNo { get; set; }
        public decimal naLaKos { get; set; }
        public string kaSaKosNo { get; set; }
        public decimal kaSaKos { get; set; }
        public string status { get; set; }
        public string jobType { get; set; }
        [ForeignKey("office")]
        public int  officeId { get; set; }
        public virtual office office { get; set; }
        public int serialNo { get; set; }
        
        public decimal sapati { get; set; }
    }
}