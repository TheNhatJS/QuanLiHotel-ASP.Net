﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLHotel.SQLHotel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QLKhachSanEntities1 : DbContext
    {
        public QLKhachSanEntities1()
            : base("name=QLKhachSanEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_bill> tbl_bill { get; set; }
        public virtual DbSet<tbl_booking> tbl_booking { get; set; }
        public virtual DbSet<tbl_room> tbl_room { get; set; }
        public virtual DbSet<tbl_user> tbl_user { get; set; }
    }
}