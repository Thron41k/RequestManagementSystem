using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RequestManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class Analog1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NomenclatureAnalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalId = table.Column<int>(type: "integer", nullable: false),
                    AnalogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureAnalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureAnalog_Nomenclature_AnalogId",
                        column: x => x.AnalogId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NomenclatureAnalog_Nomenclature_OriginalId",
                        column: x => x.OriginalId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAnalog_AnalogId",
                table: "NomenclatureAnalog",
                column: "AnalogId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAnalog_OriginalId",
                table: "NomenclatureAnalog",
                column: "OriginalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomenclatureAnalog");
        }
    }
}
