using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToAVM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcompañanteRelacion",
                table: "Citass",
                newName: "AcompananteRelacion");

            migrationBuilder.AddColumn<string>(
                name: "AcompananteAntiguedad",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteCorreo",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteDPI",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteDireccion",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteDireccionEmpresa",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteEdad",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteEmpresa",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcompananteFechaIngreso",
                table: "Pacientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcompananteFechaNacimiento",
                table: "Pacientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteNombre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteOcupacion",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteRelacion",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteTelefono",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteTelefonoEmpresa",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcompananteTipoIdentificacion",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DPIMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DPIPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEmpresaMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEmpresaPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadMadre",
                table: "Pacientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadPadre",
                table: "Pacientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpresaMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpresaPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimientoMadre",
                table: "Pacientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimientoPadre",
                table: "Pacientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OcupacionMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OcupacionPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableCorreo",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableDPI",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableDireccion",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableNit",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableNombre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsablePasaporte",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableTelefono",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoEmpresaMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoEmpresaPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoMadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoPadre",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reconsulta",
                table: "Citass",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcompananteAntiguedad",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteCorreo",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteDPI",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteDireccion",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteDireccionEmpresa",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteEdad",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteEmpresa",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteFechaIngreso",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteFechaNacimiento",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteNombre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteOcupacion",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteRelacion",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteTelefono",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteTelefonoEmpresa",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "AcompananteTipoIdentificacion",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "CorreoMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "CorreoPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DPIMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DPIPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DireccionEmpresaMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DireccionEmpresaPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DireccionMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DireccionPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "EdadMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "EdadPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "EmpresaMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "EmpresaPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "FechaNacimientoMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "FechaNacimientoPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "OcupacionMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "OcupacionPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsableCorreo",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsableDPI",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsableDireccion",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsableNit",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsableNombre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsablePasaporte",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ResponsableTelefono",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "TelefonoEmpresaMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "TelefonoEmpresaPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "TelefonoMadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "TelefonoPadre",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Reconsulta",
                table: "Citass");

            migrationBuilder.RenameColumn(
                name: "AcompananteRelacion",
                table: "Citass",
                newName: "AcompañanteRelacion");

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
    }
}
