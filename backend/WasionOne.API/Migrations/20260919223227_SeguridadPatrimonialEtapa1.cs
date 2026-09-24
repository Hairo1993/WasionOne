using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadPatrimonialEtapa1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Credencializacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoAcceso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PersonaQueVisita = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MotivoVisita = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AreaDeTrabajo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaHoraEntrada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaHoraSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credencializacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credencializacion_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recorrido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Operador = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AreaTipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: true),
                    Hallazgos = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recorrido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recorrido_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestConsigna",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    ResultadoTest = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AreaInvolucrada = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProcedimientoInvolucrado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Proveedor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConsigna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestConsigna_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credencializacion_AreaUbicacionId",
                table: "Credencializacion",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Credencializacion_IdOrigen",
                table: "Credencializacion",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrido_AreaUbicacionId",
                table: "Recorrido",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrido_IdOrigen",
                table: "Recorrido",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestConsigna_AreaUbicacionId",
                table: "TestConsigna",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestConsigna_IdOrigen",
                table: "TestConsigna",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credencializacion");

            migrationBuilder.DropTable(
                name: "Recorrido");

            migrationBuilder.DropTable(
                name: "TestConsigna");
        }
    }
}
