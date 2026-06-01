using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityTypeSelfReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseTypeId",
                table: "ActivityType",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityType_BaseTypeId",
                table: "ActivityType",
                column: "BaseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityType_ActivityType_BaseTypeId",
                table: "ActivityType",
                column: "BaseTypeId",
                principalTable: "ActivityType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityType_ActivityType_BaseTypeId",
                table: "ActivityType");

            migrationBuilder.DropIndex(
                name: "IX_ActivityType_BaseTypeId",
                table: "ActivityType");

            migrationBuilder.DropColumn(
                name: "BaseTypeId",
                table: "ActivityType");
        }
    }
}
