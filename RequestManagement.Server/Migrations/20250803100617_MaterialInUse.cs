using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class MaterialInUse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "materials_in_use",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    document_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    nomenclature_id = table.Column<int>(type: "integer", nullable: false),
                    equipment_id = table.Column<int>(type: "integer", nullable: false),
                    financially_responsible_person_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materials_in_use", x => x.id);
                    table.ForeignKey(
                        name: "FK_materials_in_use_Drivers_financially_responsible_person_id",
                        column: x => x.financially_responsible_person_id,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_materials_in_use_Equipments_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_materials_in_use_Nomenclatures_nomenclature_id",
                        column: x => x.nomenclature_id,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_materials_in_use_equipment_id",
                table: "materials_in_use",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_materials_in_use_financially_responsible_person_id",
                table: "materials_in_use",
                column: "financially_responsible_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_materials_in_use_nomenclature_id",
                table: "materials_in_use",
                column: "nomenclature_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "materials_in_use");
        }
    }
}
