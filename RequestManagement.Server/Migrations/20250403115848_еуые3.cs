using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class еуые3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Defects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DefectGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defects_DefectGroups_DefectGroupId",
                        column: x => x.DefectGroupId,
                        principalTable: "DefectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consumptions_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consumptions_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consumptions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsumptionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    DefectId = table.Column<int>(type: "integer", nullable: false),
                    ConsumptionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumptionItems_Consumptions_ConsumptionId",
                        column: x => x.ConsumptionId,
                        principalTable: "Consumptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumptionItems_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsumptionItems_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DefectGroups",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Механические повреждения" },
                    { 2, "Электрические неисправности" }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "FullName", "Position", "ShortName" },
                values: new object[,]
                {
                    { 1, "Иванов Иван Иванович", "Водитель", "Иванов И.И." },
                    { 2, "Петров Петр Петрович", "Водитель", "Петров П.П." }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Name", "StateNumber" },
                values: new object[] { 1, "КАМАЗ 53215-15", "Н 507 СН" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$abcdefghijk123456789u.lX7Qz5Z9K8zM8zM8zM8zM8zM8zM8zM8zM");

            migrationBuilder.InsertData(
                table: "Consumptions",
                columns: new[] { "Id", "Date", "DriverId", "EquipmentId", "Number", "WarehouseId" },
                values: new object[] { 1, new DateTime(2025, 4, 3, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "РСХ0001", 1 });

            migrationBuilder.InsertData(
                table: "Defects",
                columns: new[] { "Id", "DefectGroupId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Трещина корпуса" },
                    { 2, 2, "Короткое замыкание" }
                });

            migrationBuilder.InsertData(
                table: "ConsumptionItems",
                columns: new[] { "Id", "ConsumptionId", "DefectId", "NomenclatureId", "Quantity" },
                values: new object[] { 1, 1, 2, 2, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionItems_ConsumptionId",
                table: "ConsumptionItems",
                column: "ConsumptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionItems_DefectId",
                table: "ConsumptionItems",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionItems_NomenclatureId",
                table: "ConsumptionItems",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_DriverId",
                table: "Consumptions",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_EquipmentId",
                table: "Consumptions",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_Number",
                table: "Consumptions",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_WarehouseId",
                table: "Consumptions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_DefectGroupId",
                table: "Defects",
                column: "DefectGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumptionItems");

            migrationBuilder.DropTable(
                name: "Consumptions");

            migrationBuilder.DropTable(
                name: "Defects");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "DefectGroups");

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$SvaHGKIn2WlVpaLAOtF7YOM3O6mIQcatVLVDPlUVSnd5GFILMkPIq");
        }
    }
}
