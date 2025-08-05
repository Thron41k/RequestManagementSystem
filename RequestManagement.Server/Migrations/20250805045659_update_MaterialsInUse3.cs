using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class update_MaterialsInUse3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FinanciallyResponsiblePersonId",
                table: "MaterialsInUse",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateForWriteOff",
                table: "MaterialsInUse",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumberForWriteOff",
                table: "MaterialsInUse",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReasonForWriteOff",
                table: "MaterialsInUse",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateForWriteOff",
                table: "MaterialsInUse");

            migrationBuilder.DropColumn(
                name: "DocumentNumberForWriteOff",
                table: "MaterialsInUse");

            migrationBuilder.DropColumn(
                name: "ReasonForWriteOff",
                table: "MaterialsInUse");

            migrationBuilder.AlterColumn<int>(
                name: "FinanciallyResponsiblePersonId",
                table: "MaterialsInUse",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
