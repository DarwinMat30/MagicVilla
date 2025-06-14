using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaNumeroVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumerosVillas",
                columns: table => new
                {
                    VillaNumero = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    DetalleEspecial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumerosVillas", x => x.VillaNumero);
                    table.ForeignKey(
                        name: "FK_NumerosVillas_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2025, 6, 13, 23, 2, 44, 704, DateTimeKind.Local).AddTicks(1665), new DateTime(2025, 6, 13, 23, 2, 44, 704, DateTimeKind.Local).AddTicks(1679) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2025, 6, 13, 23, 2, 44, 704, DateTimeKind.Local).AddTicks(1682), new DateTime(2025, 6, 13, 23, 2, 44, 704, DateTimeKind.Local).AddTicks(1682) });

            migrationBuilder.CreateIndex(
                name: "IX_NumerosVillas_VillaId",
                table: "NumerosVillas",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumerosVillas");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6346), new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6360) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6364), new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6365) });
        }
    }
}
