using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class noId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "teamId",
                table: "Wards");

            migrationBuilder.DropColumn(
                name: "TeamIds",
                table: "Palikas");

            migrationBuilder.DropColumn(
                name: "WardIds",
                table: "Palikas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "teamId",
                table: "Wards",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

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
        }
    }
}
