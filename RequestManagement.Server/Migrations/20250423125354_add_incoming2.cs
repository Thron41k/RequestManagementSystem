using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class add_incoming2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Incoming",
                columns: new[] { "Id", "Date", "Quantity", "StockId" },
                values: new object[] { 1, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), 5m, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Incoming",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
