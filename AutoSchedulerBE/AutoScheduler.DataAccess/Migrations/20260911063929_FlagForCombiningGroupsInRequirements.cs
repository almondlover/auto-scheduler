using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FlagForCombiningGroupsInRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CombineGroups",
                table: "ActivityRequirements",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombineGroups",
                table: "ActivityRequirements");
        }
    }
}
