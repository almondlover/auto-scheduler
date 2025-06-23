using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ColumnNamings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "Availability",
                newName: "DayOfTheWeek");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DayOfTheWeek",
                table: "Availability",
                newName: "DayOfWeek");
        }
    }
}
