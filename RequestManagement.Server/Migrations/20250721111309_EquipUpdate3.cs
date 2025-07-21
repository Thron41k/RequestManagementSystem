using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class EquipUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentGroupId",
                table: "Equipments",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1,
                column: "EquipmentGroupId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentGroupId",
                table: "Equipments",
                column: "EquipmentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentGroups_EquipmentGroupId",
                table: "Equipments",
                column: "EquipmentGroupId",
                principalTable: "EquipmentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentGroups_EquipmentGroupId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_EquipmentGroupId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "EquipmentGroupId",
                table: "Equipments");
        }
    }
}
