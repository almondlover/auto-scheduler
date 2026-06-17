using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TimesheetRequirementsM2M : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreakDuration",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Timesheets",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GeneralBreakEnd",
                table: "Timesheets",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GeneralBreakStart",
                table: "Timesheets",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Timesheets",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateTable(
                name: "TimesheetActivityRequirements",
                columns: table => new
                {
                    TimesheetId = table.Column<int>(type: "int", nullable: false),
                    ActivityRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetActivityRequirements", x => new { x.TimesheetId, x.ActivityRequirementsId });
                    table.ForeignKey(
                        name: "FK_TimesheetActivityRequirements_ActivityRequirements_ActivityRequirementsId",
                        column: x => x.ActivityRequirementsId,
                        principalTable: "ActivityRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimesheetActivityRequirements_Timesheets_TimesheetId",
                        column: x => x.TimesheetId,
                        principalTable: "Timesheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetActivityRequirements_ActivityRequirementsId",
                table: "TimesheetActivityRequirements",
                column: "ActivityRequirementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimesheetActivityRequirements");

            migrationBuilder.DropColumn(
                name: "BreakDuration",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "GeneralBreakEnd",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "GeneralBreakStart",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Timesheets");
        }
    }
}
