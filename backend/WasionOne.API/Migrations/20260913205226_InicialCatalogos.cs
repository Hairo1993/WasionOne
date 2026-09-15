using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class InicialCatalogos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Direccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direccion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ubicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ubicacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DireccionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Area_Direccion_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "Direccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubArea_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubAreaUbicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubAreaId = table.Column<int>(type: "int", nullable: false),
                    UbicacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAreaUbicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubAreaUbicacion_SubArea_SubAreaId",
                        column: x => x.SubAreaId,
                        principalTable: "SubArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubAreaUbicacion_Ubicacion_UbicacionId",
                        column: x => x.UbicacionId,
                        principalTable: "Ubicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Direccion",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Administración Centralizada" },
                    { 2, "Operaciones" }
                });

            migrationBuilder.InsertData(
                table: "Ubicacion",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Planta 1" },
                    { 2, "Planta 2" },
                    { 3, "Planta 3" },
                    { 4, "CDMX - Oficina de Ventas" },
                    { 5, "Acapulco - Soporte Técnico" }
                });

            migrationBuilder.InsertData(
                table: "Area",
                columns: new[] { "Id", "DireccionId", "Nombre" },
                values: new object[,]
                {
                    { 1, 1, "Seguridad" },
                    { 2, 1, "Recursos Humanos" }
                });

            migrationBuilder.InsertData(
                table: "SubArea",
                columns: new[] { "Id", "AreaId", "Nombre" },
                values: new object[,]
                {
                    { 1, 1, "Seguridad Patrimonial" },
                    { 2, 1, "EHS" },
                    { 3, 1, "IT" },
                    { 4, 2, "RH1" },
                    { 5, 2, "RH2" }
                });

            migrationBuilder.InsertData(
                table: "SubAreaUbicacion",
                columns: new[] { "Id", "SubAreaId", "UbicacionId" },
                values: new object[,]
                {
                    { 1, 3, 1 },
                    { 2, 3, 2 },
                    { 3, 3, 3 },
                    { 4, 3, 4 },
                    { 5, 3, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_DireccionId",
                table: "Area",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubArea_AreaId",
                table: "SubArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAreaUbicacion_SubAreaId_UbicacionId",
                table: "SubAreaUbicacion",
                columns: new[] { "SubAreaId", "UbicacionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubAreaUbicacion_UbicacionId",
                table: "SubAreaUbicacion",
                column: "UbicacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubAreaUbicacion");

            migrationBuilder.DropTable(
                name: "SubArea");

            migrationBuilder.DropTable(
                name: "Ubicacion");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Direccion");
        }
    }
}
