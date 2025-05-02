using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class CodeAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Warehouses",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Article",
                table: "Nomenclature",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Equipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "");

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Code",
                value: "");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "");

            migrationBuilder.UpdateData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "");

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "");

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Code",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Drivers");

            migrationBuilder.AlterColumn<string>(
                name: "Article",
                table: "Nomenclature",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
