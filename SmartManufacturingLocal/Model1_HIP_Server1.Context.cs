﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartManufacturingLocal_HIP_PLC
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HIP_SMS_Server1Entities : DbContext
    {
        public HIP_SMS_Server1Entities()
            : base("name=HIP_SMS_Server1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Machine> Machines { get; set; }
        public virtual DbSet<Production> Productions { get; set; }
        public virtual DbSet<Setup> Setups { get; set; }
    }
}
