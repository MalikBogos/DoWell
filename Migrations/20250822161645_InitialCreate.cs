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
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Workbooks",
                columns: table => new
                {
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workbooks", x => x.WorkbookId);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkbooks",
                columns: table => new
                {
                    UserWorkbookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkbookId = table.Column<int>(type: "int", nullable: false),
                    SharedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkbooks", x => x.UserWorkbookId);
                    table.ForeignKey(
                        name: "FK_UserWorkbooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkbooks_Workbooks_WorkbookId",
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    ColumnCount = table.Column<int>(type: "int", nullable: false),
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
                    WorksheetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.CellId);
                    table.ForeignKey(
                        name: "FK_Cells_Worksheets_WorksheetId",
                        column: x => x.WorksheetId,
                        principalTable: "Worksheets",
                        principalColumn: "WorksheetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "Email", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "admin@dowell.com", "admin" },
                    { 2, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "user1@dowell.com", "user1" },
                    { 3, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "user2@dowell.com", "user2" }
                });

            migrationBuilder.InsertData(
                table: "Workbooks",
                columns: new[] { "WorkbookId", "CreatedDate", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Budget 2024" },
                    { 2, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Sales Report" }
                });

            migrationBuilder.InsertData(
                table: "UserWorkbooks",
                columns: new[] { "UserWorkbookId", "CanEdit", "SharedDate", "UserId", "WorkbookId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, true, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 3, false, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Worksheets",
                columns: new[] { "WorksheetId", "ColumnCount", "Name", "RowCount", "WorkbookId" },
                values: new object[,]
                {
                    { 1, 10, "Sheet1", 20, 1 },
                    { 2, 8, "Sheet2", 15, 1 },
                    { 3, 12, "Q1", 25, 2 }
                });

            migrationBuilder.InsertData(
                table: "Cells",
                columns: new[] { "CellId", "Column", "IsBold", "IsItalic", "IsUnderline", "Row", "Value", "WorksheetId" },
                values: new object[,]
                {
                    { 1, 0, true, false, false, 0, "Product", 1 },
                    { 2, 1, true, false, false, 0, "Price", 1 },
                    { 3, 2, true, false, false, 0, "Quantity", 1 },
                    { 4, 0, false, false, false, 1, "Laptop", 1 },
                    { 5, 1, false, false, false, 1, "999.99", 1 },
                    { 6, 2, false, false, false, 1, "5", 1 },
                    { 7, 0, false, false, false, 2, "Mouse", 1 },
                    { 8, 1, false, false, false, 2, "29.99", 1 },
                    { 9, 2, false, false, false, 2, "15", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_WorksheetId_Row_Column",
                table: "Cells",
                columns: new[] { "WorksheetId", "Row", "Column" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkbooks_UserId",
                table: "UserWorkbooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkbooks_WorkbookId",
                table: "UserWorkbooks",
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
                name: "UserWorkbooks");

            migrationBuilder.DropTable(
                name: "Worksheets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Workbooks");
        }
    }
}
