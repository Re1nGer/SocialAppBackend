using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class UserPostModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LowResMediaUrl",
                table: "UserPosts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowResMediaUrl",
                table: "UserPosts");
        }
    }
}
