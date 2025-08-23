// Data/DoWellContext.cs
using Microsoft.EntityFrameworkCore;
using DoWell.Models;
using System;

namespace DoWell.Data
{
    public class DoWellContext : DbContext
    {
        public DbSet<Cell> Cells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DoWellDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint for cell position
            modelBuilder.Entity<Cell>()
                .HasIndex(c => new { c.Row, c.Column })
                .IsUnique();

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed some sample Cells for a basic 10x10 grid
            modelBuilder.Entity<Cell>().HasData(
                new Cell { CellId = 1, Row = 0, Column = 0, Value = "Product", IsBold = true },
                new Cell { CellId = 2, Row = 0, Column = 1, Value = "Price", IsBold = true },
                new Cell { CellId = 3, Row = 0, Column = 2, Value = "Quantity", IsBold = true },
                new Cell { CellId = 4, Row = 1, Column = 0, Value = "Laptop" },
                new Cell { CellId = 5, Row = 1, Column = 1, Value = "999.99" },
                new Cell { CellId = 6, Row = 1, Column = 2, Value = "5" },
                new Cell { CellId = 7, Row = 2, Column = 0, Value = "Mouse" },
                new Cell { CellId = 8, Row = 2, Column = 1, Value = "29.99" },
                new Cell { CellId = 9, Row = 2, Column = 2, Value = "15" }
            );
        }
    }
}