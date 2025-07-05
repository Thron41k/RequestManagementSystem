using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWarehouse4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InWarehouseId",
                table: "Incomings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incomings_InWarehouseId",
                table: "Incomings",
                column: "InWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomings_Warehouses_InWarehouseId",
                table: "Incomings",
                column: "InWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomings_Warehouses_InWarehouseId",
                table: "Incomings");

            migrationBuilder.DropIndex(
                name: "IX_Incomings_InWarehouseId",
                table: "Incomings");

            migrationBuilder.DropColumn(
                name: "InWarehouseId",
                table: "Incomings");
        }
    }
}
