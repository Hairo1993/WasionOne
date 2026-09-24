using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class EtapaTresIndicadoresV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlmacenamientoServidor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Servidor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Unidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CapacidadTotalGb = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    EspacioUtilizadoGb = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    EspacioDisponibleGb = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    AlmacenamientoUtilizadoPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Umbral = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlmacenamientoServidor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlmacenamientoServidor_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisponibilidadRed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Dispositivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoDispositivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LatenciaMs = table.Column<decimal>(type: "decimal(9,2)", nullable: true),
                    PerdidaPaquetesPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TiempoCaidaMin = table.Column<int>(type: "int", nullable: true),
                    DisponibilidadPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisponibilidadRed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisponibilidadRed_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisponibilidadServidor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Servidor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Servicio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TiempoRespuestaMs = table.Column<int>(type: "int", nullable: true),
                    TiempoCaidaMin = table.Column<int>(type: "int", nullable: true),
                    DisponibilidadPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisponibilidadServidor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisponibilidadServidor_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenamientoServidor_AreaUbicacionId",
                table: "AlmacenamientoServidor",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenamientoServidor_IdOrigen",
                table: "AlmacenamientoServidor",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadRed_AreaUbicacionId",
                table: "DisponibilidadRed",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadRed_IdOrigen",
                table: "DisponibilidadRed",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadServidor_AreaUbicacionId",
                table: "DisponibilidadServidor",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadServidor_IdOrigen",
                table: "DisponibilidadServidor",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlmacenamientoServidor");

            migrationBuilder.DropTable(
                name: "DisponibilidadRed");

            migrationBuilder.DropTable(
                name: "DisponibilidadServidor");
        }
    }
}
