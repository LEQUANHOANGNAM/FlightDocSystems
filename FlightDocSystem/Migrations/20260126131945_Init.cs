using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDocSystem.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightAssigments_Flights_FlightsId",
                table: "FlightAssigments");

            migrationBuilder.DropIndex(
                name: "IX_FlightAssigments_FlightsId",
                table: "FlightAssigments");

            migrationBuilder.DropColumn(
                name: "FlightsId",
                table: "FlightAssigments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FlightsId",
                table: "FlightAssigments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightAssigments_FlightsId",
                table: "FlightAssigments",
                column: "FlightsId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightAssigments_Flights_FlightsId",
                table: "FlightAssigments",
                column: "FlightsId",
                principalTable: "Flights",
                principalColumn: "Id");
        }
    }
}
