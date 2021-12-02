using CodingExam.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CodingExam.Infrastructure.Context
{
    public class CodingExamContext : DbContext
    {
        private readonly IConfiguration _config;

        public CodingExamContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<Interest> Interests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            .LogTo(message => Debug.WriteLine(message), new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
            .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Interest>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasMany(i => i.InterestDetails).WithOne(id => id.Interest);

                entity.Property(i => i.PresentValue).IsRequired().HasColumnType("float");
                entity.Property(i => i.LowerBoundInterestRate).IsRequired().HasColumnType("float");
                entity.Property(i => i.UpperBoundInterestRate).IsRequired().HasColumnType("float");
                entity.Property(i => i.IncrementalRate).IsRequired().HasColumnType("float");
                entity.Property(i => i.MaturityYears).IsRequired();
            });

            modelBuilder.Entity<InterestDetails>(entity =>
            {
                entity.HasKey(id => id.Id);

                entity.Property(id => id.Year).IsRequired();
                entity.Property(id => id.PresentValue).IsRequired().HasColumnType("float");
                entity.Property(id => id.InterestRate).IsRequired().HasColumnType("float");
                entity.Property(id => id.FutureValue).IsRequired().HasColumnType("float");
            });
        }
    }
}