using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasionOne.API.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadEtapaDosAlcoholimetriaDopingLockers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alcoholimetria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Turno = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Positivo = table.Column<int>(type: "int", nullable: false),
                    Negativo = table.Column<int>(type: "int", nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alcoholimetria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alcoholimetria_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Turno = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NoNomina = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Resultado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doping_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroLocker = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoNomina = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AreaUbicacionId = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    HoraTermino = table.Column<TimeSpan>(type: "time", nullable: true),
                    Resultado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locker_AreaUbicacion_AreaUbicacionId",
                        column: x => x.AreaUbicacionId,
                        principalTable: "AreaUbicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alcoholimetria_AreaUbicacionId_Fecha_Turno",
                table: "Alcoholimetria",
                columns: new[] { "AreaUbicacionId", "Fecha", "Turno" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doping_AreaUbicacionId",
                table: "Doping",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Doping_IdOrigen",
                table: "Doping",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Locker_AreaUbicacionId",
                table: "Locker",
                column: "AreaUbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Locker_IdOrigen",
                table: "Locker",
                column: "IdOrigen",
                unique: true,
                filter: "[IdOrigen] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alcoholimetria");

            migrationBuilder.DropTable(
                name: "Doping");

            migrationBuilder.DropTable(
                name: "Locker");
        }
    }
}
