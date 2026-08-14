using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlmacenDesktop.Migrations
{
    /// <inheritdoc />
    public partial class MarcarCajasCerradasComoNoAbiertas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hasta v1.1.5 el cierre de caja seteaba FechaCierre pero nunca bajaba la
            // bandera EstaAbierta, así que TODAS las cajas cerradas quedaron con
            // EstaAbierta = 1. Eso dejaba el Historial de Cajas siempre vacío (filtra por
            // !EstaAbierta) y permitía que un gasto o un cobro de cuenta corriente se
            // colgara de un turno ya cerrado. Se corrige el dato histórico acá; el código
            // nuevo ya mantiene las dos columnas en sincronía.
            migrationBuilder.Sql("UPDATE Cajas SET EstaAbierta = 0 WHERE FechaCierre IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Cajas SET EstaAbierta = 1 WHERE FechaCierre IS NOT NULL;");
        }
    }
}
