using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlmacenDesktop.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionModelosIntegridad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Cajas_CajaId",
                table: "Ventas");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "CajaId",
                table: "Ventas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CAE",
                table: "Ventas",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CAEVencimiento",
                table: "Ventas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NumeroFactura",
                table: "Ventas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesAFIP",
                table: "Ventas",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PuntoVenta",
                table: "Ventas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TipoComprobante",
                table: "Ventas",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Costo",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Pagos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "MovimientosCaja",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "DetallesVenta",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "DetallesVenta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoUnitario",
                table: "DetallesCompra",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "DetallesCompra",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Compras",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalVentasOtros",
                table: "Cajas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalVentasEfectivo",
                table: "Cajas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoInicial",
                table: "Cajas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoFinalSistema",
                table: "Cajas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoFinalReal",
                table: "Cajas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Diferencia",
                table: "Cajas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "ConfiguracionesAfip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CuitEmisor = table.Column<long>(type: "INTEGER", nullable: false),
                    PuntoVenta = table.Column<int>(type: "INTEGER", nullable: false),
                    CertificadoPath = table.Column<string>(type: "TEXT", nullable: false),
                    CertificadoPassword = table.Column<string>(type: "TEXT", nullable: false),
                    EsProduccion = table.Column<bool>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Sign = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiracionToken = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesAfip", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CodigoBarras",
                table: "Productos",
                column: "CodigoBarras",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DniCuit",
                table: "Clientes",
                column: "DniCuit");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Cajas_CajaId",
                table: "Ventas",
                column: "CajaId",
                principalTable: "Cajas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Cajas_CajaId",
                table: "Ventas");

            migrationBuilder.DropTable(
                name: "ConfiguracionesAfip");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CodigoBarras",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_DniCuit",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CAE",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "CAEVencimiento",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "ObservacionesAFIP",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "PuntoVenta",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "TipoComprobante",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "DetallesVenta");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "DetallesCompra");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Compras");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Ventas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "CajaId",
                table: "Ventas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Costo",
                table: "Productos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Pagos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "MovimientosCaja",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "DetallesVenta",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoUnitario",
                table: "DetallesCompra",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalVentasOtros",
                table: "Cajas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalVentasEfectivo",
                table: "Cajas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoInicial",
                table: "Cajas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoFinalSistema",
                table: "Cajas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoFinalReal",
                table: "Cajas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Diferencia",
                table: "Cajas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Cajas_CajaId",
                table: "Ventas",
                column: "CajaId",
                principalTable: "Cajas",
                principalColumn: "Id");
        }
    }
}
