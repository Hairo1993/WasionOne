using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadEvaluacionesVigilancia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluacionVigilancia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Cobertura = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Expedientes = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Uniformidad = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Reuniones = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Equipamiento = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Supervision = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CambiosSolicitados = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Procedimientos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Capacitacion = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Acuerdos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionVigilancia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionVigilancia_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionVigilancia_AreaUbicacionId_Proveedor_Fecha",
                table: "EvaluacionVigilancia",
                columns: new[] { "AreaUbicacionId", "Proveedor", "Fecha" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionVigilancia");
        }
    }
}
