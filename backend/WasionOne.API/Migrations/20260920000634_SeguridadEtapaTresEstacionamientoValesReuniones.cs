using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadEtapaTresEstacionamientoValesReuniones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estacionamiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoMarbete = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Colaborador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    MultiPlanta = table.Column<bool>(type: "bit", nullable: false),
                    PlantasAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarcaVehiculo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmarcaVehiculo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacasVehiculo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarcaVehiculo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmarcaVehiculo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacasVehiculo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstatusDocumentacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Licencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VencimientoLicencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TarjetaCirculacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seguro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VencimientoSeguro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estacionamiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estacionamiento_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReunionProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: true),
                    Proveedor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    AsuntoMotivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Asistentes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinutaAcuerdos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReunionProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReunionProveedor_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ValeSalida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Solicitante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConceptoMotivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetalleMotivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivoFijo = table.Column<bool>(type: "bit", nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaVale = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEstimadaRetorno = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRealRetorno = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArticulosMateriales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CerradoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValeSalida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValeSalida_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estacionamiento_AreaUbicacionId",
                table: "Estacionamiento",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Estacionamiento_NoMarbete",
                table: "Estacionamiento",
                column: "NoMarbete",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReunionProveedor_AreaUbicacionId",
                table: "ReunionProveedor",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReunionProveedor_IdOrigen",
                table: "ReunionProveedor",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ValeSalida_AreaUbicacionId",
                table: "ValeSalida",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ValeSalida_IdOrigen",
                table: "ValeSalida",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estacionamiento");

            migrationBuilder.DropTable(
                name: "ReunionProveedor");

            migrationBuilder.DropTable(
                name: "ValeSalida");
        }
    }
}
