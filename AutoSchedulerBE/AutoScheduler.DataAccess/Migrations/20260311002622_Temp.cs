using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Temp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_GroupId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_ParentGroupId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ParentGroupId",
                table: "Groups");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupId",
                table: "Groups",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
