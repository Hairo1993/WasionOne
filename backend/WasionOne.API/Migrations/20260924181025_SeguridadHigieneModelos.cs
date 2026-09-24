using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadHigieneModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccidenteTrabajo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaReporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaOcurrido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Compania = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoIncidenteAccidente = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AccidenteConDiasIncapacidad = table.Column<bool>(type: "bit", nullable: false),
                    Dias = table.Column<int>(type: "int", nullable: true),
                    Lesion = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ParteLesionada = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CausaRaiz = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EstatusAccion1 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EstatusAccion2 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccidenteTrabajo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccidenteTrabajo_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Brigada",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    TipoBrigada = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NombreBrigadista = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PuestoBrigada = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaUltimaCapacitacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brigada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brigada_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CumplimientoEpp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEpp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Talla = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VidaUtilCantidad = table.Column<int>(type: "int", nullable: true),
                    VidaUtilUnidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Minimo = table.Column<int>(type: "int", nullable: true),
                    Maximo = table.Column<int>(type: "int", nullable: true),
                    Existencias = table.Column<int>(type: "int", nullable: true),
                    Solicitud = table.Column<int>(type: "int", nullable: true),
                    AbastecimientoStock = table.Column<int>(type: "int", nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CumplimientoEpp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CumplimientoEpp_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstatusLegalPlanta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    RequerimientoLegal = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Autoridad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Frecuencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UltimaFechaRealizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstatusLegalPlanta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstatusLegalPlanta_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionProveedorSegHigiene",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Proveedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Especialidad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kpi = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Cumplimiento = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionProveedorSegHigiene", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionProveedorSegHigiene_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObservacionSeguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NNomina = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonaObservada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categorias = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservacionSeguridad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservacionSeguridad_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SafetyWalk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Cumplimiento = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    AsistioGerenteCalidad = table.Column<bool>(type: "bit", nullable: false),
                    AsistioGerenteProduccion = table.Column<bool>(type: "bit", nullable: false),
                    AsistioGerenteLogistica = table.Column<bool>(type: "bit", nullable: false),
                    AsistioGerenteSoporteTecnico = table.Column<bool>(type: "bit", nullable: false),
                    AsistioGerenteProyectos = table.Column<bool>(type: "bit", nullable: false),
                    AsistioGerenteCompras = table.Column<bool>(type: "bit", nullable: false),
                    AsistioGerenteRh = table.Column<bool>(type: "bit", nullable: false),
                    AsistioCoordinadorCsh = table.Column<bool>(type: "bit", nullable: false),
                    AsistioSecretario = table.Column<bool>(type: "bit", nullable: false),
                    AsistioVocal1 = table.Column<bool>(type: "bit", nullable: false),
                    AsistioVocal2 = table.Column<bool>(type: "bit", nullable: false),
                    AsistioVocal3 = table.Column<bool>(type: "bit", nullable: false),
                    AsistioVocal4 = table.Column<bool>(type: "bit", nullable: false),
                    AsistioVocal5 = table.Column<bool>(type: "bit", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyWalk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyWalk_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccidenteTrabajo_AreaUbicacionId",
                table: "AccidenteTrabajo",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccidenteTrabajo_Folio",
                table: "AccidenteTrabajo",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brigada_AreaUbicacionId",
                table: "Brigada",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Brigada_Folio",
                table: "Brigada",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CumplimientoEpp_AreaUbicacionId",
                table: "CumplimientoEpp",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CumplimientoEpp_IdEpp_FechaRegistro",
                table: "CumplimientoEpp",
                columns: new[] { "IdEpp", "FechaRegistro" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstatusLegalPlanta_AreaUbicacionId_RequerimientoLegal_UltimaFechaRealizacion",
                table: "EstatusLegalPlanta",
                columns: new[] { "AreaUbicacionId", "RequerimientoLegal", "UltimaFechaRealizacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionProveedorSegHigiene_AreaUbicacionId",
                table: "EvaluacionProveedorSegHigiene",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionProveedorSegHigiene_Proveedor_AreaUbicacionId_Mes",
                table: "EvaluacionProveedorSegHigiene",
                columns: new[] { "Proveedor", "AreaUbicacionId", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObservacionSeguridad_AreaUbicacionId",
                table: "ObservacionSeguridad",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservacionSeguridad_Folio",
                table: "ObservacionSeguridad",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyWalk_AreaUbicacionId",
                table: "SafetyWalk",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyWalk_Fecha_AreaUbicacionId",
                table: "SafetyWalk",
                columns: new[] { "Fecha", "AreaUbicacionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccidenteTrabajo");

            migrationBuilder.DropTable(
                name: "Brigada");

            migrationBuilder.DropTable(
                name: "CumplimientoEpp");

            migrationBuilder.DropTable(
                name: "EstatusLegalPlanta");

            migrationBuilder.DropTable(
                name: "EvaluacionProveedorSegHigiene");

            migrationBuilder.DropTable(
                name: "ObservacionSeguridad");

            migrationBuilder.DropTable(
                name: "SafetyWalk");
        }
    }
}
