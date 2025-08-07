using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Article = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReasonsForWritingOffMaterialsFromOperation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonsForWritingOffMaterialsFromOperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Defects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BranchName = table.Column<string>(type: "text", nullable: false),
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
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FinanciallyResponsiblePersonId = table.Column<int>(type: "integer", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouses_Drivers_FinanciallyResponsiblePersonId",
                        column: x => x.FinanciallyResponsiblePersonId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    StateNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EquipmentGroupId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentGroups_EquipmentGroupId",
                        column: x => x.EquipmentGroupId,
                        principalTable: "EquipmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NomenclatureAnalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalId = table.Column<int>(type: "integer", nullable: false),
                    AnalogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureAnalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureAnalogs_Nomenclatures_AnalogId",
                        column: x => x.AnalogId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NomenclatureAnalogs_Nomenclatures_OriginalId",
                        column: x => x.OriginalId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SparePartsOwnerships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequiredQuantity = table.Column<int>(type: "integer", nullable: false),
                    CurrentQuantity = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    EquipmentGroupId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartsOwnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartsOwnerships_EquipmentGroups_EquipmentGroupId",
                        column: x => x.EquipmentGroupId,
                        principalTable: "EquipmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SparePartsOwnerships_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIp = table.Column<string>(type: "text", nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "text", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
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
                    LastUsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Term = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureDefectMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureDefectMappings_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NomenclatureDefectMappings_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NomenclatureDefectMappings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_Stocks_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
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
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResponsibleId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Drivers_ResponsibleId",
                        column: x => x.ResponsibleId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialsInUse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsOut = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    FinanciallyResponsiblePersonId = table.Column<int>(type: "integer", nullable: false),
                    ReasonForWriteOffId = table.Column<int>(type: "integer", nullable: false),
                    DocumentNumberForWriteOff = table.Column<string>(type: "text", nullable: false),
                    DateForWriteOff = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialsInUse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialsInUse_Drivers_FinanciallyResponsiblePersonId",
                        column: x => x.FinanciallyResponsiblePersonId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaterialsInUse_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialsInUse_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialsInUse_ReasonsForWritingOffMaterialsFromOperation_R~",
                        column: x => x.ReasonForWriteOffId,
                        principalTable: "ReasonsForWritingOffMaterialsFromOperation",
                        principalColumn: "Id");
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLastSelections_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLastSelections_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StockId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Term = table.Column<int>(type: "integer", nullable: true),
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
                name: "Incomings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    InWarehouseId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incomings_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomings_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomings_Warehouses_InWarehouseId",
                        column: x => x.InWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "DefectGroups",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "" },
                    { 2, "Выпускная система" },
                    { 3, "Гидравлика" },
                    { 4, "ДВС" },
                    { 5, "Коробка раздаточная" },
                    { 6, "Кузов, кабина" },
                    { 7, "Механизмы управления" },
                    { 8, "Рабочее оборудование" },
                    { 9, "Система охлаждения" },
                    { 10, "Сцепление" },
                    { 11, "Топливная система" },
                    { 12, "Трансмиссия" },
                    { 13, "Ходовая часть" },
                    { 14, "Электрооборудование" },
                    { 15, "Расходные материалы" },
                    { 16, "Передача в эксплуатацию" }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Code", "FullName", "Position", "ShortName" },
                values: new object[] { 1, "", "", "", "" });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Code", "EquipmentGroupId", "Name", "ShortName", "StateNumber" },
                values: new object[] { 1, "", null, "", "", "" });

            migrationBuilder.InsertData(
                table: "Nomenclatures",
                columns: new[] { "Id", "Article", "Code", "Name", "UnitOfMeasure" },
                values: new object[] { 1, "", "", "", "" });

            migrationBuilder.InsertData(
                table: "ReasonsForWritingOffMaterialsFromOperation",
                columns: new[] { "Id", "Reason" },
                values: new object[] { 1, "" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Password", "Role" },
                values: new object[] { 1, "admin", "$2a$11$IeKuyvG/5SoDYP0NFz3kouC3CPUIuUa6ShTfgVVf9oUlfqbXq8LrC", 0 });

            migrationBuilder.InsertData(
                table: "Applications",
                columns: new[] { "Id", "Date", "EquipmentId", "Number", "ResponsibleId" },
                values: new object[] { 1, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1, "", 1 });

            migrationBuilder.InsertData(
                table: "Commissions",
                columns: new[] { "Id", "ApproveForActId", "ApproveForDefectAndLimitId", "BranchName", "ChairmanId", "Member1Id", "Member2Id", "Member3Id", "Member4Id", "Name" },
                values: new object[] { 1, 1, 1, "", 1, 1, 1, 1, 1, "Могочинский филиал АО \"Труд\"" });

            migrationBuilder.InsertData(
                table: "Defects",
                columns: new[] { "Id", "DefectGroupId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "" },
                    { 2, 14, "Замена АКБ" },
                    { 3, 13, "Замена автошин" },
                    { 4, 16, "Передача в эксплуатацию" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_EquipmentId",
                table: "Applications",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Number",
                table: "Applications",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ResponsibleId",
                table: "Applications",
                column: "ResponsibleId");

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
                name: "IX_Commissions_Name",
                table: "Commissions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_DefectGroupId",
                table: "Defects",
                column: "DefectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_Name",
                table: "Defects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Code",
                table: "Drivers",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_Code",
                table: "Equipments",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentGroupId",
                table: "Equipments",
                column: "EquipmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Date",
                table: "Expenses",
                column: "Date");

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
                name: "IX_Incomings_ApplicationId",
                table: "Incomings",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomings_Date",
                table: "Incomings",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Incomings_InWarehouseId",
                table: "Incomings",
                column: "InWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomings_StockId",
                table: "Incomings",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialsInUse_EquipmentId",
                table: "MaterialsInUse",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialsInUse_FinanciallyResponsiblePersonId",
                table: "MaterialsInUse",
                column: "FinanciallyResponsiblePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialsInUse_NomenclatureId",
                table: "MaterialsInUse",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialsInUse_ReasonForWriteOffId",
                table: "MaterialsInUse",
                column: "ReasonForWriteOffId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAnalogs_AnalogId",
                table: "NomenclatureAnalogs",
                column: "AnalogId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAnalogs_OriginalId_AnalogId",
                table: "NomenclatureAnalogs",
                columns: new[] { "OriginalId", "AnalogId" });

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureDefectMappings_DefectId",
                table: "NomenclatureDefectMappings",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureDefectMappings_NomenclatureId",
                table: "NomenclatureDefectMappings",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureDefectMappings_UserId_NomenclatureId",
                table: "NomenclatureDefectMappings",
                columns: new[] { "UserId", "NomenclatureId" });

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_Code",
                table: "Nomenclatures",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_ReasonsForWritingOffMaterialsFromOperation_Reason",
                table: "ReasonsForWritingOffMaterialsFromOperation",
                column: "Reason");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartsOwnerships_EquipmentGroupId_NomenclatureId",
                table: "SparePartsOwnerships",
                columns: new[] { "EquipmentGroupId", "NomenclatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SparePartsOwnerships_NomenclatureId",
                table: "SparePartsOwnerships",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_NomenclatureId",
                table: "Stocks",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_WarehouseId_NomenclatureId",
                table: "Stocks",
                columns: new[] { "WarehouseId", "NomenclatureId" });

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

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_FinanciallyResponsiblePersonId",
                table: "Warehouses",
                column: "FinanciallyResponsiblePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Name",
                table: "Warehouses",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Incomings");

            migrationBuilder.DropTable(
                name: "MaterialsInUse");

            migrationBuilder.DropTable(
                name: "NomenclatureAnalogs");

            migrationBuilder.DropTable(
                name: "NomenclatureDefectMappings");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "SparePartsOwnerships");

            migrationBuilder.DropTable(
                name: "UserLastSelections");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "ReasonsForWritingOffMaterialsFromOperation");

            migrationBuilder.DropTable(
                name: "Defects");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Nomenclatures");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "DefectGroups");

            migrationBuilder.DropTable(
                name: "EquipmentGroups");

            migrationBuilder.DropTable(
                name: "Drivers");
        }
    }
}
