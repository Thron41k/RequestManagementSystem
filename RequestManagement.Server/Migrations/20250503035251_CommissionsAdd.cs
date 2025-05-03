using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class CommissionsAdd : Migration
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
                    Position = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StateNumber = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Article = table.Column<string>(type: "text", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
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
                name: "Commissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ApproveForActId = table.Column<int>(type: "integer", nullable: false),
                    ApproveForDefectAndLimitId = table.Column<int>(type: "integer", nullable: false),
                    ChairmanId = table.Column<int>(type: "integer", nullable: false),
                    Member1Id = table.Column<int>(type: "integer", nullable: false),
                    Member2Id = table.Column<int>(type: "integer", nullable: false),
                    Member3Id = table.Column<int>(type: "integer", nullable: false),
                    Member4Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_ApproveForActId",
                        column: x => x.ApproveForActId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_ApproveForDefectAndLimitId",
                        column: x => x.ApproveForDefectAndLimitId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_ChairmanId",
                        column: x => x.ChairmanId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_Member1Id",
                        column: x => x.Member1Id,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_Member2Id",
                        column: x => x.Member2Id,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_Member3Id",
                        column: x => x.Member3Id,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Drivers_Member4Id",
                        column: x => x.Member4Id,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    InitialQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NomenclatureDefectMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    DefectId = table.Column<int>(type: "integer", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureDefectMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureDefectMappings_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NomenclatureDefectMappings_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NomenclatureDefectMappings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLastSelections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: true),
                    EquipmentId = table.Column<int>(type: "integer", nullable: true),
                    CommissionsId = table.Column<int>(type: "integer", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLastSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLastSelections_Commissions_CommissionsId",
                        column: x => x.CommissionsId,
                        principalTable: "Commissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLastSelections_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLastSelections_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLastSelections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: true),
                    StockId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    DefectId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incoming",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incoming", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incoming_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                columns: new[] { "Id", "Code", "FullName", "Position", "ShortName" },
                values: new object[,]
                {
                    { 1, "", "Иванов Иван Иванович", "Водитель", "Иванов И.И." },
                    { 2, "", "Петров Петр Петрович", "Водитель", "Петров П.П." }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Code", "Name", "StateNumber" },
                values: new object[] { 1, "", "КАМАЗ 53215-15", "Н 507 СН" });

            migrationBuilder.InsertData(
                table: "Nomenclature",
                columns: new[] { "Id", "Article", "Code", "Name", "UnitOfMeasure" },
                values: new object[,]
                {
                    { 1, "7406.1118013", "ТКР001", "Турбокомпрессор ТКР 7С-6 левый КАМАЗ Евро 2", "шт" },
                    { 2, "6СТ-190", "АКБ001", "Аккумулятор 6СТ-190", "шт" },
                    { 3, "6СТ-200", "АКБ002", "Аккумулятор 6СТ-200 (аналог 6СТ-190)", "шт" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Password", "Role" },
                values: new object[] { 1, "admin", "$2a$11$IeKuyvG/5SoDYP0NFz3kouC3CPUIuUa6ShTfgVVf9oUlfqbXq8LrC", 0 });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Code", "LastUpdated", "Name" },
                values: new object[,]
                {
                    { 1, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Основной склад" },
                    { 2, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Резервный склад" }
                });

            migrationBuilder.InsertData(
                table: "Commissions",
                columns: new[] { "Id", "ApproveForActId", "ApproveForDefectAndLimitId", "ChairmanId", "Member1Id", "Member2Id", "Member3Id", "Member4Id", "Name" },
                values: new object[] { 1, 1, 1, 1, 1, 1, 1, 1, "Магический филиал" });

            migrationBuilder.InsertData(
                table: "Defects",
                columns: new[] { "Id", "DefectGroupId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Трещина корпуса" },
                    { 2, 2, "Короткое замыкание" }
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "ConsumedQuantity", "InitialQuantity", "NomenclatureId", "ReceivedQuantity", "WarehouseId" },
                values: new object[,]
                {
                    { 1, 0m, 70m, 1, 0m, 1 },
                    { 2, 0m, 10m, 2, 0m, 1 },
                    { 3, 0m, 40m, 1, 0m, 2 },
                    { 4, 0m, 20m, 2, 0m, 2 }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Code", "Date", "DefectId", "DriverId", "EquipmentId", "Quantity", "StockId" },
                values: new object[] { 1, "", new DateTime(2025, 4, 12, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, 1, 5m, 1 });

            migrationBuilder.InsertData(
                table: "Incoming",
                columns: new[] { "Id", "Date", "Quantity", "StockId" },
                values: new object[] { 1, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), 5m, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_ApproveForActId",
                table: "Commissions",
                column: "ApproveForActId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_ApproveForDefectAndLimitId",
                table: "Commissions",
                column: "ApproveForDefectAndLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_ChairmanId",
                table: "Commissions",
                column: "ChairmanId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_Member1Id",
                table: "Commissions",
                column: "Member1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_Member2Id",
                table: "Commissions",
                column: "Member2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_Member3Id",
                table: "Commissions",
                column: "Member3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_Member4Id",
                table: "Commissions",
                column: "Member4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_DefectGroupId",
                table: "Defects",
                column: "DefectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_DefectId",
                table: "Expenses",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_DriverId",
                table: "Expenses",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EquipmentId",
                table: "Expenses",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_StockId",
                table: "Expenses",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Incoming_StockId",
                table: "Incoming",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureDefectMappings_DefectId",
                table: "NomenclatureDefectMappings",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureDefectMappings_NomenclatureId",
                table: "NomenclatureDefectMappings",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureDefectMappings_UserId",
                table: "NomenclatureDefectMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_NomenclatureId",
                table: "Stocks",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_WarehouseId",
                table: "Stocks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastSelections_CommissionsId",
                table: "UserLastSelections",
                column: "CommissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastSelections_DriverId",
                table: "UserLastSelections",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastSelections_EquipmentId",
                table: "UserLastSelections",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastSelections_UserId",
                table: "UserLastSelections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Incoming");

            migrationBuilder.DropTable(
                name: "NomenclatureDefectMappings");

            migrationBuilder.DropTable(
                name: "UserLastSelections");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Defects");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Nomenclature");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "DefectGroups");

            migrationBuilder.DropTable(
                name: "Drivers");
        }
    }
}
