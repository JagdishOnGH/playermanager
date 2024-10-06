using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class _12111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_Teams_TeamId",
                table: "PersonalDetails");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "PersonalDetails",
                newName: "TeamsTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalDetails_TeamId",
                table: "PersonalDetails",
                newName: "IX_PersonalDetails_TeamsTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_Teams_TeamsTeamId",
                table: "PersonalDetails",
                column: "TeamsTeamId",
                principalTable: "Teams",
                principalColumn: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_Teams_TeamsTeamId",
                table: "PersonalDetails");

            migrationBuilder.RenameColumn(
                name: "TeamsTeamId",
                table: "PersonalDetails",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalDetails_TeamsTeamId",
                table: "PersonalDetails",
                newName: "IX_PersonalDetails_TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_Teams_TeamId",
                table: "PersonalDetails",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId");
        }
    }
}
