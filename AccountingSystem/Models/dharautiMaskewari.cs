using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class dharautiMaskewari
    {
        public string year { get; set; }
        public string month { get; set; }
        public int monthIndex { get; set; }
        public string nepFy { get; set; }
        public decimal prevMaujdat { get; set; }
        public decimal currentMaujdat { get; set; }
        public decimal dharautiFirta { get; set; }
        public decimal shyhaDharauti { get; set; }
        public decimal bakidharauti { get; set; }
        public decimal nagadBaki { get; set; }
        public decimal neRaBank { get; set; }
        public decimal  bankBakiMaujdat { get; set; }
    }
}