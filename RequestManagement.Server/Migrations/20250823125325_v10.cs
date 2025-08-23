using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MolForMoveId",
                table: "MaterialsInUse",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialsInUse_MolForMoveId",
                table: "MaterialsInUse",
                column: "MolForMoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialsInUse_Drivers_MolForMoveId",
                table: "MaterialsInUse",
                column: "MolForMoveId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialsInUse_Drivers_FinanciallyResponsiblePersonId",
                table: "MaterialsInUse");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialsInUse_Drivers_MolForMoveId",
                table: "MaterialsInUse");

            migrationBuilder.DropIndex(
                name: "IX_MaterialsInUse_MolForMoveId",
                table: "MaterialsInUse");

            migrationBuilder.DropColumn(
                name: "MolForMoveId",
                table: "MaterialsInUse");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialsInUse_Drivers_FinanciallyResponsiblePersonId",
                table: "MaterialsInUse",
                column: "FinanciallyResponsiblePersonId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
