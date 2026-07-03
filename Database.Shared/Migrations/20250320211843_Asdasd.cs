using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Asdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCosto",
                table: "DetallePaqueteHospitalizacion",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

       }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "PrecioCosto",
                table: "DetallePaqueteHospitalizacion");

       
        }
    }
}
