using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class userRequestDateUpdatedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserRequests",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserRequests");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserRequests");
        }
    }
}
