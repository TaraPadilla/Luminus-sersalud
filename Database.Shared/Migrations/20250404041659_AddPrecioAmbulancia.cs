using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecioAmbulancia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Ambulancias",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "198d315f-8e1b-4b2d-b2a6-0098d1dab427", "AQAAAAIAAYagAAAAEPcMmF1kocADsxZYVikMI4j6RWk143Il5gLlMSmktSIBiI0xsww2UUhtmNI0TZdMHw==", "8d1b6233-b94a-4f4d-9bc2-9752b0037aeb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0f6bbf08-522d-4994-a84a-b8dfc4da923b", "AQAAAAIAAYagAAAAEE1ed6KjcO//zsBb88KwCggW7tnLPxi2Jj3Ng6h9pOFheHraNdXSyqiFyDooQoI83Q==", "725f6395-cc54-4395-a157-f59cfb38cf2f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Ambulancias");

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
        }
    }
}
