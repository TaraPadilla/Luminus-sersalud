using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Shared.Migrations
{
    public partial class AddFechaHoraAplicacionManualSolicitudes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FechaHoraAplicacionManual",
                table: "SolicitudMedicamento",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "8b50d3cb-5be8-4e15-b4db-1b9ff073f760");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "8c3fffb7-6f8f-42f1-8124-6af4a1c94951");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "195cf4a7-9688-480e-8865-465d16ddecfc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "08854e85-b4a7-4a55-8c9c-4bd285321c5a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "e4fc843c-fc0f-4fa0-9b92-09834186b768");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "5731f567-7c2e-498f-b788-3e04eb0e02de");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "cc32e1d0-3904-4e64-a3f1-f64191a930bb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "08daac7b-cfdf-44bb-9e3b-13008be729b8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "6c45a671-be76-4301-83c5-30291a7a980c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "39dd2743-4cc0-46ae-abbe-e702c8e190b6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "c541e007-ccb0-48fb-97b1-b773452695b4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "053ef852-d64f-41f0-9efd-2a01b3db5282");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "35809d35-5ead-4dde-89c1-fd50129e052f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "42614933-d132-4209-bf2b-83c5f04d75dd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "539a2475-2968-4c3e-9e78-778f56a29f9a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "f0f03adc-1638-469b-8eba-93d17f688012");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "3eadbc29-b9fa-47df-b874-7d31e0cdd6f7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "d7cb2131-ae2e-450b-a8d7-50a82ff363aa");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "320feeb6-376b-4794-851a-b004f8859091");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "71ed5340-6b04-4a1a-8371-f4570c3d0c18");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "872dc89a-97a0-4340-adfb-76fa558d8543", "AQAAAAEAACcQAAAAEBpIPz+IUmcgWUd4b5vw0PKn3A8z6k7Aaxv/hZm+z+YgSN+0f5qtx/PncntWJW/MDQ==", "9f6bf5a9-611b-490b-924a-5a105006999a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "70558c50-8d6f-4179-a348-370e34b38814", "AQAAAAEAACcQAAAAEATac5qlyOg/SD75HJe1tqB8hbqorWIoJBywZS6xhq/yTHVzbbwAbsEIXS0ZPsyoyw==", "dc5ff8f6-c8b1-4115-9173-623bfe5ee510" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaHoraAplicacionManual",
                table: "SolicitudMedicamento");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "da9dff7f-0c7a-4e89-bac7-1843fd258c09");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "c0adc039-63bd-442d-8727-7ff711691dde");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "6f469f2a-0831-4372-a4e7-4b8cdc250136");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "ca3e5279-a4e4-40fa-b2eb-7361c938b223");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "f5faa6c2-221f-4fd0-9048-2021adedf5b3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "b3a217ca-edd4-4fb0-a668-dacddd28513b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "28eb0dbd-c335-4849-a1d1-a7082a2531e4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "62cb2316-1fcf-480b-904e-144ed3c9d84e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "3ef365c2-dfba-41a7-bae7-a19641b688fc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "a75671c4-82d0-495f-a04f-0a7362a8f0eb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "55f5bac3-4725-4f69-9fd7-edaeac127a0f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "18c5c55b-84a5-463d-94bf-ec53f2002606");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "d5a88cce-f63f-40f9-96e3-83172ff6d909");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "4342ef11-2b7a-4e38-9b10-29ac103cbe91");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "e55a61ef-c2e3-4eb6-a8fd-751c69712b36");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "3727959d-a293-4d0a-9db0-4b2d72314aef");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "b7947493-17b1-4452-8249-fd61ce60fbea");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "6b4f8529-7a10-4621-8433-99bf31491cb2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "642c0613-f7fe-41ee-8c79-bb218b0f6032");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "aaad2e85-5f33-45e4-928d-63818c97cb73");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce29e342-6348-498c-8d13-8c60cbcadb3f", "AQAAAAEAACcQAAAAELD/nerjPJSifyatSXAhCetWh0eESaJqqcBQ1hkUhs2M5nbqVEdS6N5LuW9jKk8SyA==", "7b33da6d-2291-422c-b1e7-0c9029459381" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24a7f5c5-f83e-4620-b84f-9ffae80da06f", "AQAAAAEAACcQAAAAEE11CLFg/UmKWG9W2nwasAVxbIvPoKCxMw6/LeY/tda/TUUDwFl6HT7qNHFV4xDc1Q==", "05767698-5800-47f7-bc33-fcd005533eda" });
        }
    }
}
