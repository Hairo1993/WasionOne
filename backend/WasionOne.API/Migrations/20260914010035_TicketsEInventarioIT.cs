using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class TicketsEInventarioIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioEquipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Almacen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UbicacionExacta = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TipoEquipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Hostname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Serial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ram = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Procesador = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Almacenamiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SistemaOperativo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MacWireless = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MacEthernet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UsuarioAsignado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminoGarantia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioEquipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioEquipo_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTicketOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Subcategoria = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoAsociacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Etiquetas = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Prioridad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Urgencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Impacto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EstadoAprobacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EstadoResolucion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EstadoPrimeraRespuesta = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Grupo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Agente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreSolicitante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CorreoSolicitante = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UbicacionSolicitante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SolicitanteVip = table.Column<bool>(type: "bit", nullable: false),
                    Elemento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AnyDeskEquipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartamentoOrigen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TiempoInicialRespuesta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TiempoPrimeraRespuestaHoras = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TiempoResolucionHoras = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RegistroTiempo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InteraccionesCliente = table.Column<int>(type: "int", nullable: true),
                    InteraccionesAgente = table.Column<int>(type: "int", nullable: true),
                    NotaResolucion = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResultadoEncuesta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventarioEquipo_AreaUbicacionId",
                table: "InventarioEquipo",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioEquipo_Codigo",
                table: "InventarioEquipo",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioEquipo_Serial",
                table: "InventarioEquipo",
                column: "Serial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AreaUbicacionId",
                table: "Ticket",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdTicketOrigen",
                table: "Ticket",
                column: "IdTicketOrigen",
                unique: true,
                filter: "[IdTicketOrigen] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventarioEquipo");

            migrationBuilder.DropTable(
                name: "Ticket");
        }
    }
}
