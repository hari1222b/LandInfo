using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandInfoSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBoundaryJsonToLandProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoundaryJson",
                table: "LandProperties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoundaryJson",
                table: "LandProperties");
        }
    }
}
