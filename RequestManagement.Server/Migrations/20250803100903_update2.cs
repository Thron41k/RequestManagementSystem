using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_materials_in_use_Drivers_financially_responsible_person_id",
                table: "materials_in_use");

            migrationBuilder.DropForeignKey(
                name: "FK_materials_in_use_Equipments_equipment_id",
                table: "materials_in_use");

            migrationBuilder.DropForeignKey(
                name: "FK_materials_in_use_Nomenclatures_nomenclature_id",
                table: "materials_in_use");

            migrationBuilder.DropPrimaryKey(
                name: "PK_materials_in_use",
                table: "materials_in_use");

            migrationBuilder.RenameTable(
                name: "materials_in_use",
                newName: "MaterialsInUse");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "MaterialsInUse",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "MaterialsInUse",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "MaterialsInUse",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nomenclature_id",
                table: "MaterialsInUse",
                newName: "NomenclatureId");

            migrationBuilder.RenameColumn(
                name: "financially_responsible_person_id",
                table: "MaterialsInUse",
                newName: "FinanciallyResponsiblePersonId");

            migrationBuilder.RenameColumn(
                name: "equipment_id",
                table: "MaterialsInUse",
                newName: "EquipmentId");

            migrationBuilder.RenameColumn(
                name: "document_number",
                table: "MaterialsInUse",
                newName: "DocumentNumber");

            migrationBuilder.RenameIndex(
                name: "IX_materials_in_use_nomenclature_id",
                table: "MaterialsInUse",
                newName: "IX_MaterialsInUse_NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_materials_in_use_financially_responsible_person_id",
                table: "MaterialsInUse",
                newName: "IX_MaterialsInUse_FinanciallyResponsiblePersonId");

            migrationBuilder.RenameIndex(
                name: "IX_materials_in_use_equipment_id",
                table: "MaterialsInUse",
                newName: "IX_MaterialsInUse_EquipmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialsInUse",
                table: "MaterialsInUse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialsInUse_Drivers_FinanciallyResponsiblePersonId",
                table: "MaterialsInUse",
                column: "FinanciallyResponsiblePersonId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialsInUse_Equipments_EquipmentId",
                table: "MaterialsInUse",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialsInUse_Nomenclatures_NomenclatureId",
                table: "MaterialsInUse",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
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
                name: "FK_MaterialsInUse_Equipments_EquipmentId",
                table: "MaterialsInUse");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialsInUse_Nomenclatures_NomenclatureId",
                table: "MaterialsInUse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialsInUse",
                table: "MaterialsInUse");

            migrationBuilder.RenameTable(
                name: "MaterialsInUse",
                newName: "materials_in_use");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "materials_in_use",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "materials_in_use",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "materials_in_use",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NomenclatureId",
                table: "materials_in_use",
                newName: "nomenclature_id");

            migrationBuilder.RenameColumn(
                name: "FinanciallyResponsiblePersonId",
                table: "materials_in_use",
                newName: "financially_responsible_person_id");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "materials_in_use",
                newName: "equipment_id");

            migrationBuilder.RenameColumn(
                name: "DocumentNumber",
                table: "materials_in_use",
                newName: "document_number");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialsInUse_NomenclatureId",
                table: "materials_in_use",
                newName: "IX_materials_in_use_nomenclature_id");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialsInUse_FinanciallyResponsiblePersonId",
                table: "materials_in_use",
                newName: "IX_materials_in_use_financially_responsible_person_id");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialsInUse_EquipmentId",
                table: "materials_in_use",
                newName: "IX_materials_in_use_equipment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_materials_in_use",
                table: "materials_in_use",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_materials_in_use_Drivers_financially_responsible_person_id",
                table: "materials_in_use",
                column: "financially_responsible_person_id",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_materials_in_use_Equipments_equipment_id",
                table: "materials_in_use",
                column: "equipment_id",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_materials_in_use_Nomenclatures_nomenclature_id",
                table: "materials_in_use",
                column: "nomenclature_id",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
