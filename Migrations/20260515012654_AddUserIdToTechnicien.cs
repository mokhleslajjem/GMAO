using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniGMAO.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTechnicien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Techniciens",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Techniciens",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Techniciens",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Techniciens",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Techniciens");
        }
    }
}
