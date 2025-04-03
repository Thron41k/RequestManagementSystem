using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class еуые2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityInStock",
                table: "Nomenclature",
                newName: "Receipt");

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

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Consumption", "FinalQuantity", "InitialQuantity", "Receipt" },
                values: new object[] { 0, 5, 5, 0 });

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Consumption", "FinalQuantity", "InitialQuantity", "Receipt" },
                values: new object[] { 0, 10, 10, 0 });

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Consumption", "FinalQuantity", "InitialQuantity", "Receipt" },
                values: new object[] { 0, 3, 3, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$SvaHGKIn2WlVpaLAOtF7YOM3O6mIQcatVLVDPlUVSnd5GFILMkPIq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consumption",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "FinalQuantity",
                table: "Nomenclature");

            migrationBuilder.DropColumn(
                name: "InitialQuantity",
                table: "Nomenclature");

            migrationBuilder.RenameColumn(
                name: "Receipt",
                table: "Nomenclature",
                newName: "QuantityInStock");

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 1,
                column: "QuantityInStock",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 2,
                column: "QuantityInStock",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Nomenclature",
                keyColumn: "Id",
                keyValue: 3,
                column: "QuantityInStock",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$abcdefghijk123456789u.lX7Qz5Z9K8zM8zM8zM8zM8zM8zM8zM8zM");
        }
    }
}
