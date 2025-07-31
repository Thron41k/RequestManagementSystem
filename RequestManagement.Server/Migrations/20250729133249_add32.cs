using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class add32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "SparePartsOwnerships",
                newName: "RequiredQuantity");

            migrationBuilder.AddColumn<int>(
                name: "CurrentQuantity",
                table: "SparePartsOwnerships",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentQuantity",
                table: "SparePartsOwnerships");

            migrationBuilder.RenameColumn(
                name: "RequiredQuantity",
                table: "SparePartsOwnerships",
                newName: "Quantity");
        }
    }
}
