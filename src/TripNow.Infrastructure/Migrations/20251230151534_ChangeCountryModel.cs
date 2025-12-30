using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCountryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RiskCode",
                table: "Countries",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Countries",
                newName: "RiskCode");
        }
    }
}
