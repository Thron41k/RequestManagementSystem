using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class InProgress_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclature_Warehouses_WarehouseId",
                table: "Nomenclature");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Nomenclature_WarehouseId",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "Consumption",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "FinalQuantity",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "InitialQuantity",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "Receipt",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Nomenclature");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Consumption",
                table: "Nomenclature",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FinalQuantity",
                table: "Nomenclature",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InitialQuantity",
                table: "Nomenclature",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Receipt",
                table: "Nomenclature",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "Nomenclature",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionComment = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Consumption", "FinalQuantity", "InitialQuantity", "Receipt", "WarehouseId" },
                values: new object[] { 0, 5, 5, 0, 1 });

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Consumption", "FinalQuantity", "InitialQuantity", "Receipt", "WarehouseId" },
                values: new object[] { 0, 10, 10, 0, 1 });

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Consumption", "FinalQuantity", "InitialQuantity", "Receipt", "WarehouseId" },
                values: new object[] { 0, 3, 3, 0, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_WarehouseId",
                table: "Nomenclature",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_NomenclatureId",
                table: "Items",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_RequestId",
                table: "Items",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EquipmentId",
                table: "Requests",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Number",
                table: "Requests",
                column: "Number",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclature_Warehouses_WarehouseId",
                table: "Nomenclature",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
