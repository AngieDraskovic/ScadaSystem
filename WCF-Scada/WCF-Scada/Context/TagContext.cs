using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WCF_Scada.models;

namespace WCF_Scada.Context
{
    public class TagContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<InputTag> InputTags { get; set; }
        public DbSet<OutputTag> OutputTags { get; set; }
      
        public DbSet<DigitalOutputTag> DigitalOutputTags { get; set; }
        public DbSet<AnalogInputTag> AnalogInputTags { get; set; }
        public DbSet<DigitalInputTag> DigitalInputTags { get; set; }    
        public DbSet<AnalogOutputTag> AnalogOutputTags { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Baza Tag tabeli
            modelBuilder.Entity<Tag>()
                .ToTable("Tags");

            // Izvedene tabele
            modelBuilder.Entity<InputTag>()
                .ToTable("InputTags");

            modelBuilder.Entity<OutputTag>()
                .ToTable("OutputTags");

            modelBuilder.Entity<DigitalOutputTag>()
                .ToTable("DigitalOutputTags");

            modelBuilder.Entity<AnalogOutputTag>()
                .ToTable("AnalogOutputTags");

            modelBuilder.Entity<DigitalInputTag>()
                .ToTable("DigitalInputTags");

            modelBuilder.Entity<AnalogInputTag>()
                .ToTable("AnalogInputTags");

            base.OnModelCreating(modelBuilder);
        }

    }
}