using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Shared.Migrations
{
    public partial class EditHistorialCambioHabitacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HospitalizacionCambiosHabitacion_Habitaciones_HabitacionAnt~",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.DropForeignKey(
                name: "FK_HospitalizacionCambiosHabitacion_Habitaciones_HabitacionNue~",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.DropIndex(
                name: "IX_HospitalizacionCambiosHabitacion_HabitacionAnteriorId",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.RenameColumn(
                name: "HabitacionNuevaId",
                table: "HospitalizacionCambiosHabitacion",
                newName: "HabitacionId");

            migrationBuilder.RenameColumn(
                name: "HabitacionAnteriorId",
                table: "HospitalizacionCambiosHabitacion",
                newName: "Dias");

            migrationBuilder.RenameIndex(
                name: "IX_HospitalizacionCambiosHabitacion_HabitacionNuevaId",
                table: "HospitalizacionCambiosHabitacion",
                newName: "IX_HospitalizacionCambiosHabitacion_HabitacionId");

            migrationBuilder.AddColumn<string>(
                name: "Tarifa",
                table: "HospitalizacionCambiosHabitacion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTarifa",
                table: "HospitalizacionCambiosHabitacion",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "HospitalizacionCambiosHabitacion",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "9cf33693-0e02-4fe3-b439-b0e841c6152b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "5a2e876a-17ac-4655-826b-8b6f9b671d8b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "50146441-a7c8-4e01-a375-8c3ead67e686");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "6d43bda7-2f32-4dd7-9844-ff773efd8606");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "be35699d-fbcb-459f-9bf1-b120e84ff44a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "f69e2558-76ea-4e2e-b7f6-5eec3f518d45");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "bcd1746f-f842-483e-8912-82065f2248f9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "0cb8dc45-193f-490e-a02d-0782a67df0f8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "bf51f6d3-c975-470f-9ad9-ef7c58aa3e48");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "668642f2-be64-49e1-8786-13c2dbf9ae17");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "ec1f9ec5-7325-4d13-b738-1b320f5e3437");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "b6564ca4-7127-492a-a339-98dc108d30fb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "7d6f31bb-7d69-4823-a96b-7553d6e6f552");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "283a5287-caf3-42ba-b7d6-62e783ef70e9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "764a103f-a345-4b49-8419-f8403bcdca8c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "734f15de-4da7-43e3-b90b-89f3220a4eca");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "001a1fd7-391b-4f7a-901f-8fd82e427a11");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "cd5ed640-fbe3-427c-9333-d88ec87c5433");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "e61ff8c5-59c4-468c-9ff6-dea544d88d2f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "393a0265-2d1f-45c1-8883-c9e8b9f07ab2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be10d47d-c76e-4210-99d4-7873f6baf2ea", "AQAAAAEAACcQAAAAEJXogHo/F1hXBP+8/fTfr/f0hq3RRn3soMxoxA63jUNcDVIz8ehFElaKqZzFT5Q4gA==", "a0b10001-33a1-45b6-b0b0-64e36e9af7c5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fc18b18d-bafc-4a6c-93bf-6c71a701d2d9", "AQAAAAEAACcQAAAAEAY9JubxeD/VT2iBnz8EFwNgI4del6IVSjfhaWz0HnSRwHcvV/EJFjbRmsMBVb0AmQ==", "0e335d54-811a-451d-8e3d-609ef01526de" });

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalizacionCambiosHabitacion_Habitaciones_HabitacionId",
                table: "HospitalizacionCambiosHabitacion",
                column: "HabitacionId",
                principalTable: "Habitaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HospitalizacionCambiosHabitacion_Habitaciones_HabitacionId",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.DropColumn(
                name: "Tarifa",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.DropColumn(
                name: "ValorTarifa",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "HospitalizacionCambiosHabitacion");

            migrationBuilder.RenameColumn(
                name: "HabitacionId",
                table: "HospitalizacionCambiosHabitacion",
                newName: "HabitacionNuevaId");

            migrationBuilder.RenameColumn(
                name: "Dias",
                table: "HospitalizacionCambiosHabitacion",
                newName: "HabitacionAnteriorId");

            migrationBuilder.RenameIndex(
                name: "IX_HospitalizacionCambiosHabitacion_HabitacionId",
                table: "HospitalizacionCambiosHabitacion",
                newName: "IX_HospitalizacionCambiosHabitacion_HabitacionNuevaId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0263c5c9-4a7d-44f8-bee1-51529c17d0bc",
                column: "ConcurrencyStamp",
                value: "77b17818-1347-4710-b843-ad2113555e88");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "123dfad6-796e-4d87-915b-68ae732dc4e1",
                column: "ConcurrencyStamp",
                value: "2ef60bca-d924-4d69-9bf2-dd4fb54f71f3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "124dfad6-796e-4d87-915b-68ae732dc4e2",
                column: "ConcurrencyStamp",
                value: "58aacdc3-e958-4c01-b53d-b38bf097e017");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "125dfad6-796e-4d87-915b-68ae732dc4e3",
                column: "ConcurrencyStamp",
                value: "237af318-e875-4ea9-aab7-725d64783e63");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e4",
                column: "ConcurrencyStamp",
                value: "d907707f-94af-4980-a595-275b5f162250");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e5",
                column: "ConcurrencyStamp",
                value: "e6d42353-1388-4bb6-be28-86f6e841e043");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e6",
                column: "ConcurrencyStamp",
                value: "9497e6f8-889f-4a91-87b1-aa034b35b9ff");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e8",
                column: "ConcurrencyStamp",
                value: "f0312397-a849-4cc7-9c30-7b97a0037324");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126dfad6-796e-4d87-915b-68ae732dc4e9",
                column: "ConcurrencyStamp",
                value: "801cd8f3-6984-46db-9c6b-036db258df67");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c06eaf3-10d4-40ae-bd00-3d50d629cfde",
                column: "ConcurrencyStamp",
                value: "01c8868b-2d63-4bc1-9436-366fc170f5c1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "497fdf79-d5c2-4ed9-80a8-13cee0f86b39",
                column: "ConcurrencyStamp",
                value: "4741b0f1-031f-4670-b297-90546988ca60");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e725ea5-282e-401c-9e82-da4947e382ef",
                column: "ConcurrencyStamp",
                value: "19f315fa-1493-4958-a6ba-491914f91f1d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a233184-1b23-4a52-a24f-054ee5ae2cf6",
                column: "ConcurrencyStamp",
                value: "fcfb94ff-55be-4e25-8717-9a9c83cbd2d6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725dfad6-796e-4d87-915b-68ae732dc4e7",
                column: "ConcurrencyStamp",
                value: "5efd0e67-af45-493c-9418-d896821f595c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737ff76f-c976-4ba3-bdb9-5b19fe11bbca",
                column: "ConcurrencyStamp",
                value: "a8332cbd-f27f-4f70-b45c-47927ca0ff23");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "785c160e-6a4f-4fd8-8121-4e6b2af230cd",
                column: "ConcurrencyStamp",
                value: "6fa91d71-460f-46f9-9253-a4279b10e1a0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84c54e1d-eb93-4759-ab90-d5d7007c5e55",
                column: "ConcurrencyStamp",
                value: "9cdc2ca0-6801-4b7d-9ab5-4902c191d234");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd550174-2cbe-41a7-a67b-670dfcd9d49c",
                column: "ConcurrencyStamp",
                value: "3fcd35ad-fdeb-4f84-a4ec-a38c1493c9f9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2716b4b-671d-4814-a04c-b51aa97051e8",
                column: "ConcurrencyStamp",
                value: "9423f229-f065-4b1b-9107-ca1bf76514bf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec320394-8501-4710-a2ae-85e04315a5f9",
                column: "ConcurrencyStamp",
                value: "2a604a7c-e9f1-439b-8e70-4d2f7821c285");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c286dd8-f7c8-4b2e-8e5d-a4bf811a08d4", "AQAAAAEAACcQAAAAEGeePzTQzA3nAnYCSIMP6ICpF0FwfR1f6zy2PQE9i6bpi9mUiU+A8aKtIP2MW+UPOQ==", "dcdf8a3b-ae0a-47f1-81bf-4aeef022a61f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1dcf4b23-9de9-45ab-806e-e4a0802362c0", "AQAAAAEAACcQAAAAEM4EwNVJ4fxd2Y6kPI3od5YKFhWwWN1clIrScP5v0VhcM7a0uIcCy/SQPTm15Cv2Bw==", "47aa7347-a7f8-4671-9f58-4a146c739cc2" });

            migrationBuilder.CreateIndex(
                name: "IX_HospitalizacionCambiosHabitacion_HabitacionAnteriorId",
                table: "HospitalizacionCambiosHabitacion",
                column: "HabitacionAnteriorId");

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalizacionCambiosHabitacion_Habitaciones_HabitacionAnt~",
                table: "HospitalizacionCambiosHabitacion",
                column: "HabitacionAnteriorId",
                principalTable: "Habitaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HospitalizacionCambiosHabitacion_Habitaciones_HabitacionNue~",
                table: "HospitalizacionCambiosHabitacion",
                column: "HabitacionNuevaId",
                principalTable: "Habitaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
