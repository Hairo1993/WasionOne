using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarDepartamentosYAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Area_Direccion_DireccionId",
                table: "Area");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_SubArea_SubAreaId",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "SubAreaUbicacion");

            migrationBuilder.DropTable(
                name: "SubArea");

            migrationBuilder.RenameColumn(
                name: "SubAreaId",
                table: "Usuario",
                newName: "DepartamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_SubAreaId",
                table: "Usuario",
                newName: "IX_Usuario_DepartamentoId");

            migrationBuilder.RenameColumn(
                name: "DireccionId",
                table: "Area",
                newName: "DepartamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Area_DireccionId",
                table: "Area",
                newName: "IX_Area_DepartamentoId");

            migrationBuilder.CreateTable(
                name: "AreaUbicacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    UbicacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaUbicacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaUbicacion_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreaUbicacion_Ubicacion_UbicacionId",
                        column: x => x.UbicacionId,
                        principalTable: "Ubicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DireccionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamento_Direccion_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "Direccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Seguridad Patrimonial");

            migrationBuilder.UpdateData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "EHS");

            migrationBuilder.InsertData(
                table: "Departamento",
                columns: new[] { "Id", "DireccionId", "Nombre" },
                values: new object[,]
                {
                    { 1, 1, "Seguridad" },
                    { 2, 1, "Recursos Humanos" }
                });

            migrationBuilder.InsertData(
                table: "Area",
                columns: new[] { "Id", "DepartamentoId", "Nombre" },
                values: new object[,]
                {
                    { 3, 1, "IT" },
                    { 4, 2, "RH1" },
                    { 5, 2, "RH2" }
                });

            migrationBuilder.InsertData(
                table: "AreaUbicacion",
                columns: new[] { "Id", "AreaId", "UbicacionId" },
                values: new object[,]
                {
                    { 1, 3, 1 },
                    { 2, 3, 2 },
                    { 3, 3, 3 },
                    { 4, 3, 4 },
                    { 5, 3, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaUbicacion_AreaId_UbicacionId",
                table: "AreaUbicacion",
                columns: new[] { "AreaId", "UbicacionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AreaUbicacion_UbicacionId",
                table: "AreaUbicacion",
                column: "UbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamento_DireccionId",
                table: "Departamento",
                column: "DireccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Area_Departamento_DepartamentoId",
                table: "Area",
                column: "DepartamentoId",
                principalTable: "Departamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Departamento_DepartamentoId",
                table: "Usuario",
                column: "DepartamentoId",
                principalTable: "Departamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Area_Departamento_DepartamentoId",
                table: "Area");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Departamento_DepartamentoId",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "AreaUbicacion");

            migrationBuilder.DropTable(
                name: "Departamento");

            migrationBuilder.DeleteData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.RenameColumn(
                name: "DepartamentoId",
                table: "Usuario",
                newName: "SubAreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_DepartamentoId",
                table: "Usuario",
                newName: "IX_Usuario_SubAreaId");

            migrationBuilder.RenameColumn(
                name: "DepartamentoId",
                table: "Area",
                newName: "DireccionId");

            migrationBuilder.RenameIndex(
                name: "IX_Area_DepartamentoId",
                table: "Area",
                newName: "IX_Area_DireccionId");

            migrationBuilder.CreateTable(
                name: "SubArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
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

            migrationBuilder.UpdateData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Seguridad");

            migrationBuilder.UpdateData(
                table: "Area",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Recursos Humanos");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Area_Direccion_DireccionId",
                table: "Area",
                column: "DireccionId",
                principalTable: "Direccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_SubArea_SubAreaId",
                table: "Usuario",
                column: "SubAreaId",
                principalTable: "SubArea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
