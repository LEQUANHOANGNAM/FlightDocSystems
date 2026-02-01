using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDocSystem.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Jti",
                table: "RevokedTokens",
                newName: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "RevokedTokens",
                newName: "Jti");
        }
    }
}
