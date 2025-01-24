using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USDemographicsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniqueCountyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Counties_CountyName",
                table: "Counties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountyName",
                table: "Counties",
                column: "CountyName",
                unique: true);
        }
    }
}
