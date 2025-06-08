using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "Nombre", "Ocupantes", "Superficie", "Tarifa" },
                values: new object[,]
                {
                    { 1, "Piscina, WiFi, Desayuno incluido", "Villa con vista al mar", new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6346), new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6360), "https://example.com/villa1.jpg", "Villa Real", 4, 150.5, 200.0 },
                    { 2, "Jacuzzi, Vista panorámica", "Villa en la montaña", new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6364), new DateTime(2025, 6, 8, 0, 33, 26, 494, DateTimeKind.Local).AddTicks(6365), "https://example.com/villa2.jpg", "Villa Luna", 6, 200.0, 250.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
