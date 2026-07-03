using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class MedicosSecundarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ContadorCitaAgendada",
                table: "Citass",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContadorCitaFinalizada",
                table: "Citass",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContadorCitaIniciada",
                table: "Citass",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "MedicosSecundarios",
                table: "Citass",
                type: "integer[]",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContadorCitaAgendada",
                table: "Citass");

            migrationBuilder.DropColumn(
                name: "ContadorCitaFinalizada",
                table: "Citass");

            migrationBuilder.DropColumn(
                name: "ContadorCitaIniciada",
                table: "Citass");

            migrationBuilder.DropColumn(
                name: "MedicosSecundarios",
                table: "Citass");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "637f14d2-b59b-4a3a-8c12-e561a42f6461", "AQAAAAIAAYagAAAAEAz6NpVeCSvGOwTLCRXJFNKcVf7VclJHDuKTNu3f5F8TFhKzN77Pmda5C6KcEwM7/A==", "6e088373-1467-4c03-8ae4-fff4a98e1553" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "02c1d9fe-35eb-4800-8dcb-27e0ef45b0b8", "AQAAAAIAAYagAAAAEO0dJ7/Aic54K1xbGyXsYHDBR8eyj0WFa2zpK6bPNDIkgs1DZHKnZp38xpklLj8Bow==", "c926d385-e495-472e-90ff-f122bd0ba2b9" });
        }
    }
}
