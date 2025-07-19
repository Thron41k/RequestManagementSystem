using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class add12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Equipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1,
                column: "ShortName",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Equipments");
        }
    }
}
