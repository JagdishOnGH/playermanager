using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class sthjj7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersonalDetails_RoleId",
                table: "PersonalDetails");

            migrationBuilder.DropColumn(
                name: "personId",
                table: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_RoleId",
                table: "PersonalDetails",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersonalDetails_RoleId",
                table: "PersonalDetails");

            migrationBuilder.AddColumn<int>(
                name: "personId",
                table: "Role",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_RoleId",
                table: "PersonalDetails",
                column: "RoleId",
                unique: true);
        }
    }
}
