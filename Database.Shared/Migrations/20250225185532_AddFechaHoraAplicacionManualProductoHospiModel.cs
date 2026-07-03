using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Shared.Migrations
{
    public partial class AddFechaHoraAplicacionManualProductoHospiModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FechaHoraAplicacionManual",
                table: "HospitalizacionesProductos",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "1f395257-d8e1-4d86-9e85-6e0e78031ac1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "a02c58e0-3b3f-436e-a7bb-5c79fc836083");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "f7c2962b-a274-4468-882a-46fc651993cc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "182f95e2-5bce-4a79-a2ce-b1efb5925775");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "497639b4-8fe4-476c-bc53-c7d4773cf3db");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "4850e1c1-841f-4743-a610-da64cf843540");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "1bb99914-4ef9-4bc1-a13c-ab25bc56fe32");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "0b0c3f90-503d-466e-82ec-a2f71d96fbb0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "9ede8be8-fb19-47f1-897a-b1de186ed817");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "02d149f6-4b70-41de-bd79-1f4a4af331c0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "b9ddead7-dfaa-4fd9-82dc-752026e5f098");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "f81cf72e-d0cc-41d4-a6b6-72842383f207");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "9277c6ee-25fc-4a7b-a5fd-b971250620fb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "ed1199f2-bd54-4599-9dae-cee492e4fd4d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "f41e2183-2dba-44bd-89d5-21b0d1fdc2b7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "0b0104f9-50a9-46ad-ad22-897c1f41bf86");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "4e542280-5075-4d40-a011-1f9478edf936");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "0097a036-d2fc-4afc-aa59-80a277b4cfe0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "b4f79856-af70-468c-a64d-191e844b6094");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "4baa4cfb-1bcc-410d-b8c7-b5d95028af20");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4fa4da67-3407-4e02-b10c-ef4969e9d6fb", "AQAAAAEAACcQAAAAEFwP4xrpkZcdtAWSOl3rPSrtsE88wTwcKNsxxj8uvD5eflvw6QJGPiXhi66whLYArQ==", "881ee32f-823f-4bb4-9c05-db60ecaff0e8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b379e8b0-e6f7-4c33-9ac1-cffb2dc82c0a", "AQAAAAEAACcQAAAAEIf85KaLYz/XHtX7ejQ878WooO4U/2dvd8LOz0/KXXyXmIgSwcv1wrWrNVz5QMftMw==", "850a5a83-2d7b-405f-b31d-5a8286b0c4fe" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaHoraAplicacionManual",
                table: "HospitalizacionesProductos");

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
    }
}
