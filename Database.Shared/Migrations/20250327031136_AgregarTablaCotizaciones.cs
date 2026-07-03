using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaCotizaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CotizacionesPreOrden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Items = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionesPreOrden", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e8b776c3-6cfb-4bac-8cd7-62246788d844", "AQAAAAIAAYagAAAAEKZBSgjXEXZHz0vZGRupZhojkxMSDrDzhHGf7gw1sWINhY84FoBQ25JWg4D53CvEFQ==", "5eea47bd-65fb-4f60-9fe7-0ecb2dd508e8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3a96f10e-b3c5-4a59-b687-6f8b5bb73a1c", "AQAAAAIAAYagAAAAEHD2JMGOS8EWMCkYgeGOkXIuavEawisALKfUV9WwWH3ZA0qkx01xufbbVnMzH44L5w==", "5192b6d1-3923-4620-a979-d644410b3c8f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CotizacionesPreOrden");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "243b69d3-97ba-4ef2-bea9-c9f6620d5ef0", "AQAAAAIAAYagAAAAEFZ1aBQhY2NH/z2FDK45YvILO9QdM8KYKKy5jBwKwzVJCLmsDwNgF74hf/2gqQtezQ==", "62c50ebb-57d6-400d-b6f9-ed4c697c8ec1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4fe438f5-6485-4ff6-8e1b-6592001a282d", "AQAAAAIAAYagAAAAEOWumuffzTjCLUeyspsuOYTNNNfPFH74RQa2F6sjDqQeK32UwPHVLXQaGR+7PQzrHQ==", "c55f27bc-3ce7-4c2a-9173-f50a05bec5c6" });
        }
    }
}
