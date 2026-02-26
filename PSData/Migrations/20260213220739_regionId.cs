using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSData.Migrations
{
    /// <inheritdoc />
    public partial class regionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Regiones",
                newName: "RegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "Regiones",
                newName: "Id");
        }
    }
}
