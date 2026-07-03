using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaCotizaciones2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProveedorPrincipal",
                table: "CotizacionesPreOrden",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a90c3526-7018-47a1-874f-2676abd6129c", "AQAAAAIAAYagAAAAEBiNVnAEzsUcM7uKsIjh5DMwIFUSH3+thr4KDTjlThtaq9bfJaNFXZ6EQNfOrBWkLw==", "fbb3b7bb-c1c7-4721-b188-601471819ca2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5207f4f0-37d0-46d7-a5b6-ac555ccdb76b", "AQAAAAIAAYagAAAAEDbCpTdaA//iMO3WJHp0SQwLB+Nav7DvrJSiqy5eSxt0/wFORJRm0Y0ujpIlvoA1/g==", "e925a8a4-ab7c-4cf5-b62a-989eec82a97b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProveedorPrincipal",
                table: "CotizacionesPreOrden");

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
    }
}
