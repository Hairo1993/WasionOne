using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class AdministracionEtapa1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CumplimientoDocumentacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoContrato = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MontoIvaIncluido = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    Moneda = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirmaDireccion = table.Column<bool>(type: "bit", nullable: false),
                    FirmaLegal = table.Column<bool>(type: "bit", nullable: false),
                    FirmaFinanzas = table.Column<bool>(type: "bit", nullable: false),
                    AprobadoLeninLi = table.Column<bool>(type: "bit", nullable: false),
                    Firmado = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Renovacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Carpeta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CumplimientoDocumentacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CumplimientoDocumentacion_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CumplimientoPrograma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<int>(type: "int", nullable: true),
                    AreaEvaluada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaReporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hallazgo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Seguimiento = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Prioridad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CumplimientoGeneralPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CumplimientoPrograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CumplimientoPrograma_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisponibilidadAbastecimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Material = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Especificar = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Unidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Comentarios = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisponibilidadAbastecimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisponibilidadAbastecimiento_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MantenimientoVehicular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VehiculoTipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    KilometrajeUltimoServicio = table.Column<int>(type: "int", nullable: true),
                    KilometrajeActual = table.Column<int>(type: "int", nullable: true),
                    ProximoServicio = table.Column<int>(type: "int", nullable: true),
                    Estatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MantenimientoVehicular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MantenimientoVehicular_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiempoRespuestaResolucion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraRecepcion = table.Column<TimeSpan>(type: "time", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraCierre = table.Column<TimeSpan>(type: "time", nullable: true),
                    Solicitante = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsableCompras = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Prioridad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Estatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Solicitud = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SlaCierreHoras = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    TiempoAtencionHoras = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TiempoAtencionDias = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CumplimientoSla = table.Column<bool>(type: "bit", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiempoRespuestaResolucion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiempoRespuestaResolucion_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CumplimientoDocumentacion_AreaUbicacionId",
                table: "CumplimientoDocumentacion",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CumplimientoDocumentacion_Code",
                table: "CumplimientoDocumentacion",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CumplimientoPrograma_AreaUbicacionId",
                table: "CumplimientoPrograma",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CumplimientoPrograma_IdOrigen",
                table: "CumplimientoPrograma",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DisponibilidadAbastecimiento_AreaUbicacionId",
                table: "DisponibilidadAbastecimiento",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_MantenimientoVehicular_AreaUbicacionId",
                table: "MantenimientoVehicular",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_MantenimientoVehicular_Vin",
                table: "MantenimientoVehicular",
                column: "Vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiempoRespuestaResolucion_AreaUbicacionId",
                table: "TiempoRespuestaResolucion",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TiempoRespuestaResolucion_Folio",
                table: "TiempoRespuestaResolucion",
                column: "Folio",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CumplimientoDocumentacion");

            migrationBuilder.DropTable(
                name: "CumplimientoPrograma");

            migrationBuilder.DropTable(
                name: "DisponibilidadAbastecimiento");

            migrationBuilder.DropTable(
                name: "MantenimientoVehicular");

            migrationBuilder.DropTable(
                name: "TiempoRespuestaResolucion");
        }
    }
}
