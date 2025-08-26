using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoWell.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workbooks",
                columns: table => new
                {
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSavedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workbooks", x => x.WorkbookId);
                });

            migrationBuilder.CreateTable(
                name: "FormatTemplates",
                columns: table => new
                {
                    FormatTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsBold = table.Column<bool>(type: "bit", nullable: false),
                    IsItalic = table.Column<bool>(type: "bit", nullable: false),
                    IsUnderline = table.Column<bool>(type: "bit", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    FontFamily = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FontSize = table.Column<double>(type: "float", nullable: false),
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatTemplates", x => x.FormatTemplateId);
                    table.ForeignKey(
                        name: "FK_FormatTemplates_Workbooks_WorkbookId",
                        column: x => x.WorkbookId,
                        principalTable: "Workbooks",
                        principalColumn: "WorkbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worksheets",
                columns: table => new
                {
                    WorksheetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TabOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worksheets", x => x.WorksheetId);
                    table.ForeignKey(
                        name: "FK_Worksheets_Workbooks_WorkbookId",
                        column: x => x.WorkbookId,
                        principalTable: "Workbooks",
                        principalColumn: "WorkbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    CellId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Column = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBold = table.Column<bool>(type: "bit", nullable: false),
                    IsItalic = table.Column<bool>(type: "bit", nullable: false),
                    IsUnderline = table.Column<bool>(type: "bit", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForegroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormatTemplateId = table.Column<int>(type: "int", nullable: true),
                    WorksheetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.CellId);
                    table.ForeignKey(
                        name: "FK_Cells_FormatTemplates_FormatTemplateId",
                        column: x => x.FormatTemplateId,
                        principalTable: "FormatTemplates",
                        principalColumn: "FormatTemplateId");
                    table.ForeignKey(
                        name: "FK_Cells_Worksheets_WorksheetId",
                        column: x => x.WorksheetId,
                        principalTable: "Worksheets",
                        principalColumn: "WorksheetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Workbooks",
                columns: new[] { "WorkbookId", "Author", "CreatedDate", "FilePath", "LastSavedDate", "Name" },
                values: new object[] { 1, "DoWell User", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Sample Workbook" });

            migrationBuilder.InsertData(
                table: "FormatTemplates",
                columns: new[] { "FormatTemplateId", "BackgroundColor", "FontFamily", "FontSize", "ForegroundColor", "IsBold", "IsItalic", "IsUnderline", "Name", "WorkbookId" },
                values: new object[,]
                {
                    { 1, "#4472C4", "Segoe UI", 12.0, "#FFFFFF", true, false, false, "Header Style", 1 },
                    { 2, "#F2F2F2", "Consolas", 11.0, "#0000FF", false, false, false, "Number Style", 1 },
                    { 3, "#FFFF00", "Segoe UI", 11.0, "#000000", true, false, false, "Highlight Style", 1 }
                });

            migrationBuilder.InsertData(
                table: "Worksheets",
                columns: new[] { "WorksheetId", "CreatedDate", "ModifiedDate", "Name", "TabOrder", "WorkbookId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Sheet1", 1, 1 },
                    { 2, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Sheet2", 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Cells",
                columns: new[] { "CellId", "BackgroundColor", "Column", "ForegroundColor", "FormatTemplateId", "IsBold", "IsItalic", "IsUnderline", "Row", "Value", "WorksheetId" },
                values: new object[,]
                {
                    { 1, "#4472C4", 0, "#FFFFFF", 1, true, false, false, 0, "Product", 1 },
                    { 2, "#4472C4", 1, "#FFFFFF", 1, true, false, false, 0, "Price", 1 },
                    { 3, "#4472C4", 2, "#FFFFFF", 1, true, false, false, 0, "Quantity", 1 },
                    { 4, "#FFFFFF", 0, "#000000", null, false, false, false, 1, "Laptop", 1 },
                    { 5, "#F2F2F2", 1, "#0000FF", 2, false, false, false, 1, "999.99", 1 },
                    { 6, "#F2F2F2", 2, "#0000FF", 2, false, false, false, 1, "5", 1 },
                    { 7, "#FFFFFF", 0, "#000000", null, false, false, false, 2, "Mouse", 1 },
                    { 8, "#F2F2F2", 1, "#0000FF", 2, false, false, false, 2, "29.99", 1 },
                    { 9, "#F2F2F2", 2, "#0000FF", 2, false, false, false, 2, "15", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_FormatTemplateId",
                table: "Cells",
                column: "FormatTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_WorksheetId_Row_Column",
                table: "Cells",
                columns: new[] { "WorksheetId", "Row", "Column" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormatTemplates_WorkbookId",
                table: "FormatTemplates",
                column: "WorkbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Worksheets_WorkbookId",
                table: "Worksheets",
                column: "WorkbookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "FormatTemplates");

            migrationBuilder.DropTable(
                name: "Worksheets");

            migrationBuilder.DropTable(
                name: "Workbooks");
        }
    }
}
