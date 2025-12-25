using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlmacenDesktop.Migrations
{
    /// <inheritdoc />
    public partial class cambiosenlasclases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockMinimo",
                table: "Productos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "StockMinimo",
                table: "Productos");
        }
    }
}
