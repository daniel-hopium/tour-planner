using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using TourPlanner.Persistence.Entities;


namespace TourPlanner.Persistence.Utils
{
    public class TourPlannerDbContext : DbContext
    {
        public DbSet<TourEntity> Tours { get; set; }
        public DbSet<TourLogEntity> TourLogs { get; set; }
        public DbSet<AddressEntity> Addresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(DatabaseManager.ConnectionString)
                .EnableDetailedErrors()
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TourEntity>()
                .HasOne(t => t.FromAddress)
                .WithMany()
                .HasForeignKey(t => t.FromAddressId);

            modelBuilder.Entity<TourEntity>()
                .HasOne(t => t.ToAddress)
                .WithMany()
                .HasForeignKey(t => t.ToAddressId);
            modelBuilder.Entity<TourLogEntity>()
                .HasOne(t => t.Tour)
                .WithMany(t => t.Logs)
                .HasForeignKey(t => t.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
    }
}
