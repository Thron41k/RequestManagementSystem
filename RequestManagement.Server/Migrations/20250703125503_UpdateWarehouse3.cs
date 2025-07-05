using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWarehouse3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinanciallyResponsiblePersonId",
                table: "Warehouses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_FinanciallyResponsiblePersonId",
                table: "Warehouses",
                column: "FinanciallyResponsiblePersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Drivers_FinanciallyResponsiblePersonId",
                table: "Warehouses",
                column: "FinanciallyResponsiblePersonId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Drivers_FinanciallyResponsiblePersonId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_FinanciallyResponsiblePersonId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "FinanciallyResponsiblePersonId",
                table: "Warehouses");
        }
    }
}
