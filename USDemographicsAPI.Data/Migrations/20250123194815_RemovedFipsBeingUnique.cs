using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USDemographicsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFipsBeingUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_States_StateFips",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_Counties_CountyFips",
                table: "Counties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_States_StateFips",
                table: "States",
                column: "StateFips",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountyFips",
                table: "Counties",
                column: "CountyFips",
                unique: true);
        }
    }
}
