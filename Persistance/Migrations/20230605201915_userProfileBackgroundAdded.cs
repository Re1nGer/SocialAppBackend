using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class userProfileBackgroundAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileBackgroundImagelink",
                table: "Users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileBackgroundImagelink",
                table: "Users");
        }
    }
}
