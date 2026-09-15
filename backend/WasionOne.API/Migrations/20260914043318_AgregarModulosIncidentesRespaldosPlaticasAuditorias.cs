using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModulosIncidentesRespaldosPlaticasAuditorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditoriaEquipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRealizada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Almacen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoActivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DescripcionActivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Revisados = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaEquipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriaEquipo_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidenteCritico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFallaOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: true),
                    DuracionHoras = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DepartamentoId = table.Column<int>(type: "int", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Linea = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Severidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Causa = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Detalles = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Contramedida = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenteCritico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenteCritico_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenteCritico_Departamento_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Platica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemaPolitica = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ResponsableEnvio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MedioDifusion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Platica_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Respaldo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaRespaldo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SistemaAplicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoftwareUtilizado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoRespaldo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respaldo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Respaldo_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaEquipo_AreaUbicacionId",
                table: "AuditoriaEquipo",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaEquipo_Folio",
                table: "AuditoriaEquipo",
                column: "Folio",
                unique: true,
                filter: "[Folio] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenteCritico_AreaUbicacionId",
                table: "IncidenteCritico",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenteCritico_DepartamentoId",
                table: "IncidenteCritico",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenteCritico_IdFallaOrigen",
                table: "IncidenteCritico",
                column: "IdFallaOrigen",
                unique: true,
                filter: "[IdFallaOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Platica_AreaUbicacionId",
                table: "Platica",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Platica_IdOrigen",
                table: "Platica",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Respaldo_AreaUbicacionId",
                table: "Respaldo",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Respaldo_IdOrigen",
                table: "Respaldo",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriaEquipo");

            migrationBuilder.DropTable(
                name: "IncidenteCritico");

            migrationBuilder.DropTable(
                name: "Platica");

            migrationBuilder.DropTable(
                name: "Respaldo");
        }
    }
}
