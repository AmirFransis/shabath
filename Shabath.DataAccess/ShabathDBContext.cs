using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shabath.DataAccess.Models;
using Microsoft.Extensions.Configuration;

namespace Shabath.DataAccess
{
    public class ShabathDBContext : DbContext
    {

        public ShabathDBContext(DbContextOptions<ShabathDBContext> options) : base(options)
        {
        }
        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<Rounds> Rounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Members>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IsChosenForThisWeek).HasColumnType("bit");
                entity.Property(e => e.IsActive).HasColumnType("bit");
            });

            modelBuilder.Entity<Rounds>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EventDate).HasColumnType("date");
                entity.Property(e => e.DayOfWeek).HasColumnType("navchar");
            });
        }
    }
}
