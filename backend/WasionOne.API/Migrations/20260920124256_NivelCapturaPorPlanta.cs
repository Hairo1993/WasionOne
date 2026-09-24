using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class NivelCapturaPorPlanta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioModuloUbicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ModuloClave = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioModuloUbicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioModuloUbicacion_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioModuloUbicacion_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioModuloUbicacion_AreaUbicacionId",
                table: "UsuarioModuloUbicacion",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioModuloUbicacion_UsuarioId_ModuloClave_AreaUbicacionId",
                table: "UsuarioModuloUbicacion",
                columns: new[] { "UsuarioId", "ModuloClave", "AreaUbicacionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioModuloUbicacion");
        }
    }
}
