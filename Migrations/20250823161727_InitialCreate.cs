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
                    IsUnderline = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.CellId);
                });

            migrationBuilder.InsertData(
                table: "Cells",
                columns: new[] { "CellId", "Column", "IsBold", "IsItalic", "IsUnderline", "Row", "Value" },
                values: new object[,]
                {
                    { 1, 0, true, false, false, 0, "Product" },
                    { 2, 1, true, false, false, 0, "Price" },
                    { 3, 2, true, false, false, 0, "Quantity" },
                    { 4, 0, false, false, false, 1, "Laptop" },
                    { 5, 1, false, false, false, 1, "999.99" },
                    { 6, 2, false, false, false, 1, "5" },
                    { 7, 0, false, false, false, 2, "Mouse" },
                    { 8, 1, false, false, false, 2, "29.99" },
                    { 9, 2, false, false, false, 2, "15" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_Row_Column",
                table: "Cells",
                columns: new[] { "Row", "Column" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cells");
        }
    }
}
