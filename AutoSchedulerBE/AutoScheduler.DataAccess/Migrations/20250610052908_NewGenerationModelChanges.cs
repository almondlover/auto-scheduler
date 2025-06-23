using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewGenerationModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Timeslots_TimeslotId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TimeslotId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TimeslotId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "DayOfWeek",
                table: "Timeslots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Timeslots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_GroupId",
                table: "Timeslots",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_OrganizationId",
                table: "Members",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Halls_OrganizationId",
                table: "Halls",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Halls_Organizations_OrganizationId",
                table: "Halls",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Organizations_OrganizationId",
                table: "Members",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslots_Groups_GroupId",
                table: "Timeslots",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Halls_Organizations_OrganizationId",
                table: "Halls");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Organizations_OrganizationId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Timeslots_Groups_GroupId",
                table: "Timeslots");

            migrationBuilder.DropIndex(
                name: "IX_Timeslots_GroupId",
                table: "Timeslots");

            migrationBuilder.DropIndex(
                name: "IX_Members_OrganizationId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Halls_OrganizationId",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Timeslots");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "Timeslots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TimeslotId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TimeslotId",
                table: "Groups",
                column: "TimeslotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Timeslots_TimeslotId",
                table: "Groups",
                column: "TimeslotId",
                principalTable: "Timeslots",
                principalColumn: "Id");
        }
    }
}
