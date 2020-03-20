using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AccountingSystem.Models;

namespace AccountingSystem.DAL
{
 public class AccountContext : DbContext
    {
        public AccountContext()
            : base("mycon")
        {
        }
        public DbSet<officer> officers { get; set; }
        public DbSet<office> offices { get; set; }
        public DbSet<talabiBharpai> talabiBharpais { get; set; }
        public DbSet<yearMonth> yearMonths { get; set; }
        public DbSet<baUSiNa> baUSiNas { get; set; }
        public DbSet<khaSiNa> khaSiNas { get; set; }
        public DbSet<khaShrot> khaShrots { get; set; }
        public DbSet<bhuktaniAdesh> bhuktaniAdeshs { get; set; }
        public DbSet<jornalEntries> jornalEntries { get; set; }
        public DbSet<bhuktaniPaune> bhuktaniPaunes { get; set; }
        public DbSet<byehora> byehoras { get; set; }
        public DbSet<fiscalYear> fisYears { get; set; }
        public DbSet<setNumber> setNumbers { get; set; }
        //public DbSet<budget> budgets { get; set; }
        public DbSet<arthaBudget> arthaBudgets { get; set; }
        //public DbSet<bankNagadi> bankNagadis { get; set; }
        //public DbSet<fantwariKharcha> fantwariKharchas { get; set; }
        //public DbSet<bajetHisab> bajetHisabs { get; set; }
        public DbSet<rajaswoAsuli> rajaswoAsulis { get; set; }
        public DbSet<rajaswoDakhila> rajaswoDakhilas { get; set; }
        public DbSet<rajaswoSirsak> rajaswoSirsaks { get; set; }
        public DbSet<rajaswoUpasirsak> rajaswoUpasirsaks { get; set; }
        public DbSet<vendor> vendors { get; set; }
        public DbSet<dharautiNaamNamasi> dharautiNaamNamasis { get; set; }
        public DbSet<ayaKar> ayaKars { get; set; }
        public DbSet<dharautiAmdani> dharautiAmdanis { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<officer>().MapToStoredProcedures();
            modelBuilder.Entity<office>().MapToStoredProcedures();
            
        }
    }

}