using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarUbicacionesCatalogoIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ubicacion",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "CDMX");

            migrationBuilder.InsertData(
                table: "Ubicacion",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 6, "Monterrey" },
                    { 7, "Planta 5" }
                });

            migrationBuilder.InsertData(
                table: "AreaUbicacion",
                columns: new[] { "Id", "AreaId", "UbicacionId" },
                values: new object[,]
                {
                    { 6, 3, 6 },
                    { 7, 3, 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AreaUbicacion",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AreaUbicacion",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ubicacion",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ubicacion",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "Ubicacion",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "CDMX - Oficina de Ventas");
        }
    }
}
