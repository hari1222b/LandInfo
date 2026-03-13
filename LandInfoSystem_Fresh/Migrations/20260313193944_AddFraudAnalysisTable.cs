using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandInfoSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFraudAnalysisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FraudAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    FraudScore = table.Column<int>(type: "int", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDuplicate = table.Column<bool>(type: "bit", nullable: false),
                    IsPriceAnomaly = table.Column<bool>(type: "bit", nullable: false),
                    IsSuspiciousSeller = table.Column<bool>(type: "bit", nullable: false),
                    WarningMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudAnalyses_LandProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "LandProperties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FraudAnalyses_PropertyId",
                table: "FraudAnalyses",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FraudAnalyses");
        }
    }
}
