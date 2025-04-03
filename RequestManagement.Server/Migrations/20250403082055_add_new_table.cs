using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class add_new_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_Number",
                table: "Requests");

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Article = table.Column<string>(type: "text", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: false),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nomenclature_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NomenclatureAnalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MainNomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    AnalogNomenclatureId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureAnalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureAnalogs_Nomenclature_AnalogNomenclatureId",
                        column: x => x.AnalogNomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NomenclatureAnalogs_Nomenclature_MainNomenclatureId",
                        column: x => x.MainNomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$abcdefghijk123456789u.lX7Qz5Z9K8zM8zM8zM8zM8zM8zM8zM8zM");

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Основной склад" },
                    { 2, "Резервный склад" }
                });

            migrationBuilder.InsertData(
                table: "Nomenclature",
                columns: new[] { "Id", "Article", "Code", "Name", "QuantityInStock", "UnitOfMeasure", "WarehouseId" },
                values: new object[,]
                {
                    { 1, "7406.1118013", "ТКР001", "Турбокомпрессор ТКР 7С-6 левый КАМАЗ Евро 2", 5, "шт", 1 },
                    { 2, "6СТ-190", "АКБ001", "Аккумулятор 6СТ-190", 10, "шт", 1 },
                    { 3, "6СТ-200", "АКБ002", "Аккумулятор 6СТ-200 (аналог 6СТ-190)", 3, "шт", 2 }
                });

            migrationBuilder.InsertData(
                table: "NomenclatureAnalogs",
                columns: new[] { "Id", "AnalogNomenclatureId", "MainNomenclatureId" },
                values: new object[] { 1, 3, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Number",
                table: "Requests",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_WarehouseId",
                table: "Nomenclature",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAnalogs_AnalogNomenclatureId",
                table: "NomenclatureAnalogs",
                column: "AnalogNomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAnalogs_MainNomenclatureId",
                table: "NomenclatureAnalogs",
                column: "MainNomenclatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomenclatureAnalogs");

            migrationBuilder.DropTable(
                name: "Nomenclature");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Number",
                table: "Requests");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$OYodlvyV7aKmnyJRb/C6JO6Lc4drw06z8UReFQl2xhp4ejxyW67IS");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Number",
                table: "Requests",
                column: "Number");
        }
    }
}
