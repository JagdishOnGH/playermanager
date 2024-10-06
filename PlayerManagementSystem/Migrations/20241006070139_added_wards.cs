using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class added_wards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssociationId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamOf",
                table: "Teams");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "PalikaId",
                table: "Teams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WardId",
                table: "Teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "TeamIds",
                table: "Palikas",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<Guid[]>(
                name: "WardIds",
                table: "Palikas",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    wardNo = table.Column<int>(type: "integer", nullable: false),
                    teamId = table.Column<int[]>(type: "integer[]", nullable: false),
                    PalikaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Palikas_PalikaId",
                        column: x => x.PalikaId,
                        principalTable: "Palikas",
                        principalColumn: "PalikaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_PalikaId",
                table: "Teams",
                column: "PalikaId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_WardId",
                table: "Teams",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_PalikaId",
                table: "Wards",
                column: "PalikaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Palikas_PalikaId",
                table: "Teams",
                column: "PalikaId",
                principalTable: "Palikas",
                principalColumn: "PalikaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Wards_WardId",
                table: "Teams",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Palikas_PalikaId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Wards_WardId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropIndex(
                name: "IX_Teams_PalikaId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_WardId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "PalikaId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamIds",
                table: "Palikas");

            migrationBuilder.DropColumn(
                name: "WardIds",
                table: "Palikas");

            migrationBuilder.AddColumn<int>(
                name: "AssociationId",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamOf",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
