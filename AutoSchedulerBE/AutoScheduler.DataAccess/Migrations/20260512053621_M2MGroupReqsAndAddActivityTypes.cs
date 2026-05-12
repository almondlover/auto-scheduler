using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class M2MGroupReqsAndAddActivityTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityRequirements_Groups_GroupId",
                table: "ActivityRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ActivityRequirements_GroupId",
                table: "ActivityRequirements");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ActivityRequirements");

            migrationBuilder.AddColumn<int>(
                name: "ActivityTypeId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityRequirementsGroup",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    ActivityRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRequirementsGroup", x => new { x.GroupId, x.ActivityRequirementsId });
                    table.ForeignKey(
                        name: "FK_ActivityRequirementsGroup_ActivityRequirements_ActivityRequirementsId",
                        column: x => x.ActivityRequirementsId,
                        principalTable: "ActivityRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityRequirementsGroup_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityTypeId",
                table: "Activities",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRequirementsGroup_ActivityRequirementsId",
                table: "ActivityRequirementsGroup",
                column: "ActivityRequirementsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityType_ActivityTypeId",
                table: "Activities",
                column: "ActivityTypeId",
                principalTable: "ActivityType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityType_ActivityTypeId",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "ActivityRequirementsGroup");

            migrationBuilder.DropTable(
                name: "ActivityType");

            migrationBuilder.DropIndex(
                name: "IX_Activities_ActivityTypeId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "Activities");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ActivityRequirements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRequirements_GroupId",
                table: "ActivityRequirements",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityRequirements_Groups_GroupId",
                table: "ActivityRequirements",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
