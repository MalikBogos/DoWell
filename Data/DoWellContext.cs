// Data/DoWellContext.cs
using Microsoft.EntityFrameworkCore;
using DoWell.Models;
using System;

namespace DoWell.Data
{
    public class DoWellContext : DbContext
    {
        public DbSet<Workbook> Workbooks { get; set; }
        public DbSet<Worksheet> Worksheets { get; set; }
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

            // Configure relationships - FIX CASCADE DELETE ISSUE
            modelBuilder.Entity<Workbook>()
                .HasMany(w => w.Worksheets)
                .WithOne(ws => ws.Workbook)
                .HasForeignKey(ws => ws.WorkbookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Worksheet>()
                .HasMany(ws => ws.Cells)
                .WithOne(c => c.Worksheet)
                .HasForeignKey(c => c.WorksheetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Workbook>()
                .HasMany(w => w.FormatTemplates)
                .WithOne(ft => ft.Workbook)
                .HasForeignKey(ft => ft.WorkbookId)
                .OnDelete(DeleteBehavior.Cascade);

            // BELANGRIJK: Geen cascade delete voor FormatTemplate -> Cells om cycles te voorkomen
            modelBuilder.Entity<FormatTemplate>()
                .HasMany(ft => ft.Cells)
                .WithOne(c => c.FormatTemplate)
                .HasForeignKey(c => c.FormatTemplateId)
                .OnDelete(DeleteBehavior.NoAction); // CHANGED FROM SetNull to NoAction

            // Unique constraint for cell position within a worksheet
            modelBuilder.Entity<Cell>()
                .HasIndex(c => new { c.WorksheetId, c.Row, c.Column })
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

            // Seed Worksheets
            modelBuilder.Entity<Worksheet>().HasData(
                new Worksheet
                {
                    WorksheetId = 1,
                    Name = "Sheet1",
                    WorkbookId = 1,
                    TabOrder = 1,
                    CreatedDate = seedDate,
                    ModifiedDate = seedDate
                },
                new Worksheet
                {
                    WorksheetId = 2,
                    Name = "Sheet2",
                    WorkbookId = 1,
                    TabOrder = 2,
                    CreatedDate = seedDate,
                    ModifiedDate = seedDate
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
                    WorksheetId = 1,
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
                    WorksheetId = 1,
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
                    WorksheetId = 1,
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
                    WorksheetId = 1
                },
                new Cell
                {
                    CellId = 5,
                    Row = 1,
                    Column = 1,
                    Value = "999.99",
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    WorksheetId = 1,
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
                    WorksheetId = 1,
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
                    WorksheetId = 1
                },
                new Cell
                {
                    CellId = 8,
                    Row = 2,
                    Column = 1,
                    Value = "29.99",
                    BackgroundColor = "#F2F2F2",
                    ForegroundColor = "#0000FF",
                    WorksheetId = 1,
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
                    WorksheetId = 1,
                    FormatTemplateId = 2
                }
            );
        }
    }
}