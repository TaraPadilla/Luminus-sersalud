using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Asd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadAplicada",
                table: "HospitalizacionDetallePaqueteHospitalizacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadExcedida",
                table: "HospitalizacionDetallePaqueteHospitalizacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3930879a-7d57-407f-8293-6dffbdd4bb2f", "AQAAAAIAAYagAAAAELhSITNnO4ki9YIvJU6SPRZQ3J6zB0muACuNPk5c9epqETiCs3FJzuVjfMAWr7U3kQ==", "fe516042-a1fd-4c1a-bf74-1ae0ba9c4968" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "395de668-b77f-4d05-a714-1920429bfb9b", "AQAAAAIAAYagAAAAEHcGysNQ+ZZrcHmfxjQH/WDhZtiWL4OhAOPkEZoiG31z1Jgm4Fsr8Dl7NhhUmMFI1g==", "9d609518-0d55-4489-b2cb-5338bc5061b1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadAplicada",
                table: "HospitalizacionDetallePaqueteHospitalizacion");

            migrationBuilder.DropColumn(
                name: "CantidadExcedida",
                table: "HospitalizacionDetallePaqueteHospitalizacion");

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
    }
}
