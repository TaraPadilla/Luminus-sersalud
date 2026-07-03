using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Shared.Migrations
{
    public partial class AddFechaTraslado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaTraslado",
                table: "DetalleTraslado",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "24ed7889-afe8-4c4b-aae3-114f783bb24f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "599e681e-8c02-4f22-b89c-9ee2eba84560");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "37454777-23d1-4f87-9834-ba87b892d09a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "6fc5566b-8ca2-4d1d-bb37-f76afafd940f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "2a182cc1-efbd-4233-b7f2-0ae895c58995");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "54b73559-6393-4f2c-8505-912ae8c962e5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "335b66b1-6ae8-4369-97b3-35ebf3707d25");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "f4e94a8b-86bb-46ce-8da5-0c8c53dd00c1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "6ea0882d-f8d4-412b-808d-f172c77cd54e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "6f007e83-5397-4484-8709-3078387de586");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "cbd8524f-7d0c-4cf3-ad02-19af69117a7c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "40a690fb-f4ab-4434-84ca-a8a435b7da19");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "7ca4cb82-ae7b-47e5-95de-559c8ac13069");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "45aea9c4-0d98-402c-bccf-94bfec30465d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "ae73cab0-f3b3-4594-92de-9966567a7c9b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "cb3bb599-a4ff-4e86-a5a7-3e722a386d78");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "6ab220a6-69cb-486d-abb1-4e049e691a80");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "35eb7937-3322-4f92-a651-ff6d7dc95407");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "66abacc3-3afd-437b-84ef-a02320666759");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "cd282b7a-f7ad-4797-b819-f1c7f3c02717");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6964352c-c73a-4d15-9080-8d3e3b13fcce", "AQAAAAEAACcQAAAAEH+RWNF2qnbXaTHmzHR61XG2TMQl9QsjsIjJmhcQgzw6yGo0V4vd8ypIws6X4MwJZA==", "bd53e2f3-5d35-407b-b414-958ff0569aeb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "88e4edc6-5635-4917-89df-7f55928d5fa3", "AQAAAAEAACcQAAAAEN+gzo9phqZz93pQrg7oc+Al0/zmNIixm0p1x9dFFbTNL/ETuaPqPLmrfKf+oYqNzg==", "7207751d-51d9-485a-bcff-fbe1e3f43143" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaTraslado",
                table: "DetalleTraslado");

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
    }
}
