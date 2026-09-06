using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class HallListForRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityRequirementsHall",
                columns: table => new
                {
                    HallId = table.Column<int>(type: "int", nullable: false),
                    ActivityRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRequirementsHall", x => new { x.HallId, x.ActivityRequirementsId });
                    table.ForeignKey(
                        name: "FK_ActivityRequirementsHall_ActivityRequirements_ActivityRequirementsId",
                        column: x => x.ActivityRequirementsId,
                        principalTable: "ActivityRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityRequirementsHall_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRequirementsHall_ActivityRequirementsId",
                table: "ActivityRequirementsHall",
                column: "ActivityRequirementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRequirementsHall");
        }
    }
}
