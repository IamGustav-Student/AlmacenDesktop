using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlmacenDesktop.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarFiadoACuentaCorriente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // "Fiado" se renombró a "Cuenta Corriente" en la UI — actualizamos el
            // valor ya guardado en Ventas.MetodoPago para que las pantallas de
            // Cuenta Corriente (que filtran por este literal) sigan encontrando
            // las ventas fiadas cargadas antes de este cambio.
            migrationBuilder.Sql("UPDATE Ventas SET MetodoPago = 'Cuenta Corriente' WHERE MetodoPago = 'Fiado';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Ventas SET MetodoPago = 'Fiado' WHERE MetodoPago = 'Cuenta Corriente';");
        }
    }
}
