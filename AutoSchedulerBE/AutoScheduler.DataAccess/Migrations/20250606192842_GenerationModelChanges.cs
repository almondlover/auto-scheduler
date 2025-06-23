using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class GenerationModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "ActivityRequirements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    HallId = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availability_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Availability_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRequirements_MemberId",
                table: "ActivityRequirements",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_HallId",
                table: "Availability",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_MemberId",
                table: "Availability",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityRequirements_Members_MemberId",
                table: "ActivityRequirements",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityRequirements_Members_MemberId",
                table: "ActivityRequirements");

            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropIndex(
                name: "IX_ActivityRequirements_MemberId",
                table: "ActivityRequirements");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "ActivityRequirements");
        }
    }
}
