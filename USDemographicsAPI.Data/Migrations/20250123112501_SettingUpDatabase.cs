using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USDemographicsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class SettingUpDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    StateAbbreviation = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    StateFips = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CountyFips = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Population = table.Column<int>(type: "int", nullable: false),
                    PopulationPerSquareMile = table.Column<double>(type: "float", nullable: false),
                    SquareMiles = table.Column<double>(type: "float", nullable: false),
                    ShapeArea = table.Column<double>(type: "float", nullable: false),
                    ShapeLength = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Counties_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountyFips",
                table: "Counties",
                column: "CountyFips",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountyName",
                table: "Counties",
                column: "CountyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counties_StateId",
                table: "Counties",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_States_StateAbbreviation",
                table: "States",
                column: "StateAbbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_StateFips",
                table: "States",
                column: "StateFips",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_StateName",
                table: "States",
                column: "StateName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
