// Data/DoWellContext.cs
using Microsoft.EntityFrameworkCore;
using DoWell.Models;
using System;

namespace DoWell.Data
{
    public class DoWellContext : DbContext
    {
        public DbSet<Workbook> Workbooks { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<FormatTemplate> FormatTemplates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(
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

            // Configure relationships
            modelBuilder.Entity<Workbook>()
                .HasMany(w => w.Cells)
                .WithOne(c => c.Workbook)
                .HasForeignKey(c => c.WorkbookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Workbook>()
                .HasMany(w => w.FormatTemplates)
                .WithOne(ft => ft.Workbook)
                .HasForeignKey(ft => ft.WorkbookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Geen cascade delete voor FormatTemplate -> Cells om cycles te voorkomen
            modelBuilder.Entity<FormatTemplate>()
                .HasMany(ft => ft.Cells)
                .WithOne(c => c.FormatTemplate)
                .HasForeignKey(c => c.FormatTemplateId)
                .OnDelete(DeleteBehavior.NoAction);

            // Unique constraint voor cell positie binnen een workbook
            modelBuilder.Entity<Cell>()
                .HasIndex(c => new { c.WorkbookId, c.Row, c.Column })
                .IsUnique();

            // Seed data
            var seedDate = new DateTime(2025, 1, 1, 12, 0, 0);

            // Seed Workbook
            modelBuilder.Entity<Workbook>().HasData(
                new Workbook
                {
                    WorkbookId = 1,
                    Name = "Sample Workbook",
                    Author = "DoWell User",
                    CreatedDate = seedDate,
                    LastSavedDate = seedDate
                }
            );

            // Seed Format Templates
            modelBuilder.Entity<FormatTemplate>().HasData(
                new FormatTemplate
                {
                    FormatTemplateId = 1,
                    Name = "Header Style",
                    WorkbookId = 1,
                    IsBold = true,
                    BackgroundColor = "#4472C4",
                    ForegroundColor = "#FFFFFF",
                    FontSize = 12,
                    FontFamily = "Segoe UI"
                },
                new FormatTemplate
                {
                    FormatTemplateId = 2,
                    Name = "Number Style",
                    WorkbookId = 1,
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    FontFamily = "Consolas",
                    FontSize = 11
                },
                new FormatTemplate
                {
                    FormatTemplateId = 3,
                    Name = "Highlight Style",
                    WorkbookId = 1,
                    BackgroundColor = "#FFFF00",
                    ForegroundColor = "#000000",
                    IsBold = true,
                    FontFamily = "Segoe UI",
                    FontSize = 11
                }
            );

            // Seed Cells
            modelBuilder.Entity<Cell>().HasData(
                new Cell
                {
                    CellId = 1,
                    Row = 0,
                    Column = 0,
                    Value = "Product",
                    IsBold = true,
                    BackgroundColor = "#4472C4",
                    ForegroundColor = "#FFFFFF",
                    WorkbookId = 1,
                    FormatTemplateId = 1
                },
                new Cell
                {
                    CellId = 2,
                    Row = 0,
                    Column = 1,
                    Value = "Price",
                    IsBold = true,
                    BackgroundColor = "#4472C4",
                    ForegroundColor = "#FFFFFF",
                    WorkbookId = 1,
                    FormatTemplateId = 1
                },
                new Cell
                {
                    CellId = 3,
                    Row = 0,
                    Column = 2,
                    Value = "Quantity",
                    IsBold = true,
                    BackgroundColor = "#4472C4",
                    ForegroundColor = "#FFFFFF",
                    WorkbookId = 1,
                    FormatTemplateId = 1
                },
                new Cell
                {
                    CellId = 4,
                    Row = 1,
                    Column = 0,
                    Value = "Laptop",
                    BackgroundColor = "#FFFFFF",
                    ForegroundColor = "#000000",
                    WorkbookId = 1
                },
                new Cell
                {
                    CellId = 5,
                    Row = 1,
                    Column = 1,
                    Value = "999.99",
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    WorkbookId = 1,
                    FormatTemplateId = 2
                },
                new Cell
                {
                    CellId = 6,
                    Row = 1,
                    Column = 2,
                    Value = "5",
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    WorkbookId = 1,
                    FormatTemplateId = 2
                },
                new Cell
                {
                    CellId = 7,
                    Row = 2,
                    Column = 0,
                    Value = "Mouse",
                    BackgroundColor = "#FFFFFF",
                    ForegroundColor = "#000000",
                    WorkbookId = 1
                },
                new Cell
                {
                    CellId = 8,
                    Row = 2,
                    Column = 1,
                    Value = "29.99",
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    WorkbookId = 1,
                    FormatTemplateId = 2
                },
                new Cell
                {
                    CellId = 9,
                    Row = 2,
                    Column = 2,
                    Value = "15",
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    WorkbookId = 1,
                    FormatTemplateId = 2
                }
            );
        }
    }
}