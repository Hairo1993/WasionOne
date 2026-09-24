using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class RolSuperadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsuarioModulo",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UsuarioModulo",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UsuarioModulo",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UsuarioModulo",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UsuarioModulo",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UsuarioModulo",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "Rol",
                value: "Superadmin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "Rol",
                value: "CEO");

            migrationBuilder.InsertData(
                table: "UsuarioModulo",
                columns: new[] { "Id", "ModuloClave", "UsuarioId" },
                values: new object[,]
                {
                    { 1, "it.tickets", 1 },
                    { 2, "it.inventario", 1 },
                    { 3, "it.incidentes", 1 },
                    { 4, "it.respaldos", 1 },
                    { 5, "it.platicas", 1 },
                    { 6, "it.auditorias", 1 }
                });
        }
    }
}
