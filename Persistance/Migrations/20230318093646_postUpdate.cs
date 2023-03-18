using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class postUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserPostId",
                table: "UserLikes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_UserPostId",
                table: "UserLikes",
                column: "UserPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_UserPosts_UserPostId",
                table: "UserLikes",
                column: "UserPostId",
                principalTable: "UserPosts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_UserPosts_UserPostId",
                table: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserLikes_UserPostId",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "UserPostId",
                table: "UserLikes");
        }
    }
}
