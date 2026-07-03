using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class CreateAmbulanciasRelationHospi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalizacionId",
                table: "Ambulancias",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b0521b4-445b-483b-9417-e5888e127cf0", "AQAAAAIAAYagAAAAEJhTXB2hqwAMKOujZU0j6e7+wDu0u1/27jbZOKOS7voZhzjrHEWnmVZzDcO8qHpleQ==", "c5ef04e3-17e5-4364-8924-4e3a5d8c59b8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a83292e5-86a5-473f-a32e-6c8cb3cdcce8", "AQAAAAIAAYagAAAAEBfdA5LJZo73jRB70F3setUUrHOg1CX1QLrdp4sn3LLKn8hmiKbqvymJB+0iGCivyA==", "9541c094-4ba2-4fe2-9d72-2de5bfb5112a" });

            migrationBuilder.CreateIndex(
                name: "IX_Ambulancias_HospitalizacionId",
                table: "Ambulancias",
                column: "HospitalizacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ambulancias_Hospitalizaciones_HospitalizacionId",
                table: "Ambulancias",
                column: "HospitalizacionId",
                principalTable: "Hospitalizaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ambulancias_Hospitalizaciones_HospitalizacionId",
                table: "Ambulancias");

            migrationBuilder.DropIndex(
                name: "IX_Ambulancias_HospitalizacionId",
                table: "Ambulancias");

            migrationBuilder.DropColumn(
                name: "HospitalizacionId",
                table: "Ambulancias");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24bc75df-06c0-4cac-ab92-5e6687ce3a1e", "AQAAAAIAAYagAAAAEBf1LCDC0ZLqv8fg52BYljJFsQHjCHGmYemmbPgSRQURM09h0uOYvpMl0II2qLMcsw==", "18b363a7-5397-4bee-a617-ffee284f3c17" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd51d712-70c3-42bc-86c5-ee9aa0ee4be6", "AQAAAAIAAYagAAAAEARF5MU913hDPpGt044zF/anFnJDarOiwW6KaFn/fOxyvEBd5Nspvl8GxzR6nf7gaQ==", "3b114f79-6cb1-46db-b9ed-c22b18bae379" });
        }
    }
}
