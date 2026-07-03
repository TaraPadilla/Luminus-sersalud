using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Shared.Migrations
{
    public partial class AddObservacionesDetalleExamen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacion",
                table: "DetalleExamenes",
                type: "text",
                nullable: true);

           
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "8c668fcc-e435-4d0c-ad0d-42aa7dcc4300");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "724cbf1b-3fb1-49c7-b3e0-b57a9c4d2760");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "5b5d8c7b-7c56-4cfd-91da-7d25b19c0987");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "6fd2a1e5-d469-4378-8dcc-0dd775c678a1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "6e4715b1-a97b-4314-91b7-51fdac428865");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "01069619-23fa-4b8f-88b3-14484f65cf64");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "4ae5c959-85b5-47d2-9f6c-7be5f7158cc0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "1749f7e8-2891-45f9-bd76-e54521a62428");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "ed75decd-00d7-4f9f-90b2-7d4d53880802");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "c6ff50ac-bbbc-4b9d-8117-f1be9da1fb60");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "fcf08a7a-8e52-4ec1-a66b-df5786f21eff");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "fa8c7e90-c03c-4c9b-9e4f-76ea7e4e5f3a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "022726f3-addf-4737-9cfe-d039a77bc3f8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "39fa5cd5-2157-4772-97aa-06dd3a359dba");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "bb0c52c5-6360-4a28-b755-3769e4f97320");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "dad2ddae-d3e2-4b7b-9901-bd7da2e9cde3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "16235759-f5db-4635-8d78-7382b19c75d4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "58258c58-56ab-4d73-9aec-dabea5a936af");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "1d03cbb9-b19e-41c9-a393-734906484004");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "c58ab9c8-dd93-455a-9589-9764d72aea90");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fef5dc4a-0d9d-412f-9f03-c11591802d27", "AQAAAAEAACcQAAAAEC/96SQ6nLLYXuaSlfiTzr1cwVrDsQElsY0koY19hAk8CJHmy4pfoy6o9liWETPRrw==", "c9fdecc6-b784-4bbe-8acb-595e86b09d83" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "32c6105a-f693-40a5-9a01-fc139993c8d3", "AQAAAAEAACcQAAAAEFuMOHnvWXai3K6VRnocD622HVvXZXlARzQQwvkUH0BOHI9TigbM9z9aknyWhHKstw==", "c953cc37-0f43-430f-83cb-7df6fe4a769a" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
       

            migrationBuilder.DropColumn(
                name: "Observacion",
                table: "DetalleExamenes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "f2114559-462e-4738-b129-db71fd507969");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "a0094a71-680f-407d-89d7-78f8fd64b3d1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "0ee22aca-514a-41dc-8b8c-b9d3291b0893");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "362b72f0-c472-4166-bdce-bbdc062804b1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "3d626859-1811-4f1b-b910-cb14e249833b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "493f3888-ae7d-4242-8775-7c5eb9aa25c0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "098cd426-9d93-46c1-9dca-2ec6bdc4b067");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "8ed053e7-4f97-49ee-b53c-d82ef41878b0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "88f22d00-f775-4306-9358-a6b26a80344c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "345759cb-2d7d-4d90-a4ab-5803516f9341");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "c87dd80c-ab11-4ba0-88bc-0200cea57bbf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "11ef3a06-e131-445f-a15a-0f086ade0b9e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "363e0914-7e01-4928-a64d-f7badbbaa7c4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "561deec7-5008-492a-89a2-ca6849c8a57c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "c0c7405f-a6c0-494c-a4cd-237b131a3ffb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "31ffdf20-ddeb-418d-bfe7-22752495e147");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "8fb5d08f-698e-4ad1-8f28-17e1e7752d40");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "6d285535-aabf-4c74-bcbd-d2f560fa275a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "93c30ef0-8b46-45ff-afdf-3f624a80f294");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "e968e812-c675-4632-b79d-fae732b8682d");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ac559787-beb4-451e-a0a9-6c1b26dcf223", "AQAAAAEAACcQAAAAEOrzmpKwW21qbTFbS1Q+HhaSXZewQDExuNwobGfPFsE9q2ZwgnzeMDiosZjFte3RMQ==", "e1a798b5-649b-4634-85b8-80f6ac68a3c7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cfb376a8-518e-4cae-a127-42cc02d6aca7", "AQAAAAEAACcQAAAAEClW824XpvgQwZGM1CEAKQqiGiGPGWCCn8plhAPjdVS88Ni7MZOPFpjptjcENXkIJA==", "badcef04-e350-4fe4-b5dd-7c1b09d99e94" });
        }
    }
}
