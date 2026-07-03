using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Shared.Migrations
{
    public partial class AddCamposEmpleado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioLabores",
                table: "Empleados",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JornadaTrabajo",
                table: "Empleados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VacacionesProgramadas",
                table: "Empleados",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "3c2ccd9b-942b-4b68-8f0d-13e4c7bc985a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "ef38740f-a828-453b-9a35-c5b9201a956b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "7ee8ef09-b15b-4382-9032-104419337fd1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "63a06082-c790-4232-bc47-324a15589e67");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "3fa55c4d-2847-46ca-8847-a7eed32da381");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "5fc68acb-961d-461b-801d-83869ef5a9b3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "914bdc37-8740-4bfd-aae7-73ef1dad3f5b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "240c1ce1-da9d-4d6c-8ca4-773865fb4e62");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "099083fb-756f-4f88-af99-a9bc3a2e0b2f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "84bf7aac-785a-412b-97ff-878ec5cc82ef");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "25734c27-4126-40fc-ab41-c6c9537bf1b8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "560ba90f-1b26-4190-adc3-1b506bc293c2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "60d4edde-1c91-4e53-94d8-eaefe76587d7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "ed36849e-f059-4d3e-a540-eaebed306614");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "56001f28-b6b6-4a2d-aaf1-25863c5e393e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "24b2ffcd-c03d-47b2-822f-fe9d44a7248f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "68811daa-789a-4c6f-be5a-a8fda5798d91");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "ee9054cd-5ccd-4550-8de1-c84be3535b91");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "a725c6a6-e1bc-42d7-8756-825ced1dd820");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "d281516d-8a06-4f29-983d-00dda1599cc4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "17e8a0ed-6906-4dc0-a086-2a3f7f22558a", "AQAAAAEAACcQAAAAEB5FNS+zsTdk0DO5O6YmdSiGV4lVyQkiMqq269ivlPWYSc4LlbFDYBmaqSBxcEq/6w==", "49361b19-703c-4ff9-8ea6-57d616e2523c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d4db0cf7-0da8-4eef-8ca2-ed7218cc5364", "AQAAAAEAACcQAAAAEHX1631ERsJ5CV2InJJsUX3MqNmBHdgmq9B6FEyWA2tWkwBtouoYO29kb618u5Y9zw==", "f81c11ae-591d-49c0-8f3a-14e326ea55ce" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaInicioLabores",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "JornadaTrabajo",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "VacacionesProgramadas",
                table: "Empleados");

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
    }
}
