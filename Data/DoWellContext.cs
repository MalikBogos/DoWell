// Data/DoWellContext.cs
using Microsoft.EntityFrameworkCore;
using DoWell.Models;
using System;
using System.Linq;

namespace DoWell.Data
{
    public class DoWellContext : DbContext
    {
        public DbSet<Workbook> Workbooks { get; set; }
        public DbSet<Worksheet> Worksheets { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserWorkbook> UserWorkbooks { get; set; }

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
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Worksheet>()
                .HasOne(w => w.Workbook)
                .WithMany(wb => wb.Worksheets)
                .HasForeignKey(w => w.WorkbookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cell>()
                .HasOne(c => c.Worksheet)
                .WithMany(ws => ws.Cells)
                .HasForeignKey(c => c.WorksheetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkbook>()
                .HasOne(uw => uw.User)
                .WithMany(u => u.UserWorkbooks)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkbook>()
                .HasOne(uw => uw.Workbook)
                .WithMany()
                .HasForeignKey(uw => uw.WorkbookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for cell position
            modelBuilder.Entity<Cell>()
                .HasIndex(c => new { c.WorksheetId, c.Row, c.Column })
                .IsUnique();

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Use static dates for seeding to avoid migration warnings
            var seedDate = new DateTime(2024, 1, 1, 10, 0, 0);

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "admin", Email = "admin@dowell.com", CreatedDate = seedDate },
                new User { UserId = 2, Username = "user1", Email = "user1@dowell.com", CreatedDate = seedDate },
                new User { UserId = 3, Username = "user2", Email = "user2@dowell.com", CreatedDate = seedDate }
            );

            // Seed Workbooks
            modelBuilder.Entity<Workbook>().HasData(
                new Workbook { WorkbookId = 1, Name = "Budget 2024", CreatedDate = seedDate, ModifiedDate = seedDate },
                new Workbook { WorkbookId = 2, Name = "Sales Report", CreatedDate = seedDate, ModifiedDate = seedDate }
            );

            // Seed Worksheets
            modelBuilder.Entity<Worksheet>().HasData(
                new Worksheet { WorksheetId = 1, Name = "Sheet1", WorkbookId = 1, RowCount = 20, ColumnCount = 10 },
                new Worksheet { WorksheetId = 2, Name = "Sheet2", WorkbookId = 1, RowCount = 15, ColumnCount = 8 },
                new Worksheet { WorksheetId = 3, Name = "Q1", WorkbookId = 2, RowCount = 25, ColumnCount = 12 }
            );

            // Seed some sample Cells
            modelBuilder.Entity<Cell>().HasData(
                new Cell { CellId = 1, WorksheetId = 1, Row = 0, Column = 0, Value = "Product", IsBold = true },
                new Cell { CellId = 2, WorksheetId = 1, Row = 0, Column = 1, Value = "Price", IsBold = true },
                new Cell { CellId = 3, WorksheetId = 1, Row = 0, Column = 2, Value = "Quantity", IsBold = true },
                new Cell { CellId = 4, WorksheetId = 1, Row = 1, Column = 0, Value = "Laptop" },
                new Cell { CellId = 5, WorksheetId = 1, Row = 1, Column = 1, Value = "999.99" },
                new Cell { CellId = 6, WorksheetId = 1, Row = 1, Column = 2, Value = "5" },
                new Cell { CellId = 7, WorksheetId = 1, Row = 2, Column = 0, Value = "Mouse" },
                new Cell { CellId = 8, WorksheetId = 1, Row = 2, Column = 1, Value = "29.99" },
                new Cell { CellId = 9, WorksheetId = 1, Row = 2, Column = 2, Value = "15" }
            );

            // Seed UserWorkbooks (sharing workbooks)
            modelBuilder.Entity<UserWorkbook>().HasData(
                new UserWorkbook { UserWorkbookId = 1, UserId = 1, WorkbookId = 1, SharedDate = seedDate, CanEdit = true },
                new UserWorkbook { UserWorkbookId = 2, UserId = 1, WorkbookId = 2, SharedDate = seedDate, CanEdit = true },
                new UserWorkbook { UserWorkbookId = 3, UserId = 2, WorkbookId = 1, SharedDate = seedDate, CanEdit = false }
            );
        }
    }
}