using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class setNumber
    {[Key]
        public int setNumberId { get; set; }
        public string faramName { get; set; }
        public int baUSiNaId { get; set; }
        public virtual baUSiNa  baUSiNa { get; set; }
        public int number { get; set; }

    }
}