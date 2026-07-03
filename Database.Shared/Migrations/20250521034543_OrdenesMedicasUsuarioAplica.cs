using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class OrdenesMedicasUsuarioAplica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRealizacion",
                table: "OrdenesMedicas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProfesionalRealiza",
                table: "OrdenesMedicas",
                type: "text",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRealizacion",
                table: "OrdenesMedicas");

            migrationBuilder.DropColumn(
                name: "ProfesionalRealiza",
                table: "OrdenesMedicas");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3fc99642-7ad8-44d2-bd58-9cec87e524bd", "AQAAAAIAAYagAAAAEDvMSb3xnKQdKO7fxNKVCkw4arhyyd0g+tKKFek6QTCZO1rgpSW6g8aXWXUDARJC6w==", "44ebc9b1-7615-49e2-a065-cf8092e42b44" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "efa5250b-afb6-442f-aacb-67420e835f62", "AQAAAAIAAYagAAAAELP0kY6xTtjBuZq2RQBvJxCGZ9GBPmBC3ZwrNYiNc59mltIFuei9/qxTmJLmwgQkUw==", "9c69543f-7141-4d39-88a5-8053572c5f0a" });
        }
    }
}
