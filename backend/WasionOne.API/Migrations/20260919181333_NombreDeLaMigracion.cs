using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioModulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ModuloClave = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioModulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioModulo_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioModulo_UsuarioId_ModuloClave",
                table: "UsuarioModulo",
                columns: new[] { "UsuarioId", "ModuloClave" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioModulo");
        }
    }
}
