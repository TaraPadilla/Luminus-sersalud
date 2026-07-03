using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ñ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f79c9d6-ff5e-4d0f-90c0-5f7610114999", "AQAAAAIAAYagAAAAEB0pOcBiMyK+of1R1bbh5eF2XGyOo0oybWFHWK0CpK6bKPoFpo9jhSPXInXdEI35Gg==", "5cb04502-4f53-4b34-b947-a62f2536559c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e0825cbe-2699-4f2b-89a5-5b1fbb017ce8", "AQAAAAIAAYagAAAAEDSGq0kbu0muNMEPyDne1pKUavEvhX3mTRXCzmRkAbrIb5cw99Yvu3sVjnliz0Cotg==", "c351122b-e1cc-4f51-8227-54da645c9c6c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c1fe9bf7-ae71-41fd-9037-89771967eb2b", "AQAAAAIAAYagAAAAEBu3cNgJyiFozvc8renbupCMRyoX4gLCOq2ZEE5GAeUm1B/EEod+MDe0led1bF1OWQ==", "f42cc9fb-e1eb-4ae0-85d1-e521ee4079c3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c3d61b02-6314-48e4-94de-d640612836b1", "AQAAAAIAAYagAAAAEJMy99DGQU5TGTvHzkSaBjkdrzDf6NA3lg3WcPo8b4SAb3dAFO1uz4+atBDxR9xqqA==", "046e4e4b-9d4d-433d-83bd-901ff1b3e7b7" });
        }
    }
}
