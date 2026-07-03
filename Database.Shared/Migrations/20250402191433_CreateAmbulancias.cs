using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class CreateAmbulancias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ambulancias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServicioSolicitado = table.Column<string>(type: "text", nullable: true),
                    FechaHoraSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NombrePaciente = table.Column<string>(type: "text", nullable: true),
                    Cama = table.Column<string>(type: "text", nullable: true),
                    EsTraslado = table.Column<bool>(type: "boolean", nullable: false),
                    EsEgreso = table.Column<bool>(type: "boolean", nullable: false),
                    DireccionTraslado = table.Column<string>(type: "text", nullable: true),
                    FormaConduccion = table.Column<string>(type: "text", nullable: true),
                    TipoViaje = table.Column<string>(type: "text", nullable: true),
                    HoraSalidaAmbulancia = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HoraEntradaAmbulancia = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AmbulanciaUnidad = table.Column<string>(type: "text", nullable: true),
                    NombrePiloto = table.Column<string>(type: "text", nullable: true),
                    ExamenConsulta = table.Column<string>(type: "text", nullable: true),
                    AfiliacionIGSS = table.Column<string>(type: "text", nullable: true),
                    HoraExamen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnfermeraAcompania = table.Column<bool>(type: "boolean", nullable: false),
                    Oxigeno = table.Column<bool>(type: "boolean", nullable: false),
                    CantidadOxigeno = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ambulancias", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24bc75df-06c0-4cac-ab92-5e6687ce3a1e", "AQAAAAIAAYagAAAAEBf1LCDC0ZLqv8fg52BYljJFsQHjCHGmYemmbPgSRQURM09h0uOYvpMl0II2qLMcsw==", "18b363a7-5397-4bee-a617-ffee284f3c17" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd51d712-70c3-42bc-86c5-ee9aa0ee4be6", "AQAAAAIAAYagAAAAEARF5MU913hDPpGt044zF/anFnJDarOiwW6KaFn/fOxyvEBd5Nspvl8GxzR6nf7gaQ==", "3b114f79-6cb1-46db-b9ed-c22b18bae379" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ambulancias");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3930879a-7d57-407f-8293-6dffbdd4bb2f", "AQAAAAIAAYagAAAAELhSITNnO4ki9YIvJU6SPRZQ3J6zB0muACuNPk5c9epqETiCs3FJzuVjfMAWr7U3kQ==", "fe516042-a1fd-4c1a-bf74-1ae0ba9c4968" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dc6916f0-8d11-43a3-b143-37ce429bee33",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "395de668-b77f-4d05-a714-1920429bfb9b", "AQAAAAIAAYagAAAAEHcGysNQ+ZZrcHmfxjQH/WDhZtiWL4OhAOPkEZoiG31z1Jgm4Fsr8Dl7NhhUmMFI1g==", "9d609518-0d55-4489-b2cb-5338bc5061b1" });
        }
    }
}
