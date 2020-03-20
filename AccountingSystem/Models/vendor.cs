using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class vendor
    {
        [Key]
        public int vendorId { get; set; }
        [Required]
        public string name { get; set; }
        public string address { get; set; }
        public string TPIN_PAN { get; set; }
        [Display(Name = "kar Data No")]
        public string karDataNo { get; set; }
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [DataType(DataType.EmailAddress)]
        public string altEmail { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone No.")]
        public string phone { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile No.")]
        public string mobilePhone { get; set; }
        [DataType(DataType.Url)]
        public string url { get; set; }
        [Display(Name = "Contact Person")]
        public string contactPerson { get; set; }
        public string bankName { get; set; }
        public string bankBranch { get; set; }
        public string bankAccNo { get; set; }
       
    }
}