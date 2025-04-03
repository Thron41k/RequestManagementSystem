using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NomenclatureId",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Items_NomenclatureId",
                table: "Items",
                column: "NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Nomenclature_NomenclatureId",
                table: "Items",
                column: "NomenclatureId",
                principalTable: "Nomenclature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Nomenclature_NomenclatureId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_NomenclatureId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "NomenclatureId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Items",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
