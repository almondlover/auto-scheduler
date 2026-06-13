using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TimesheetStateInPlaceOfIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Timesheets");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Timesheets");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Timesheets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
