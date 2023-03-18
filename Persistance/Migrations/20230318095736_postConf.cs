using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    public partial class postConf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserComments_UserPosts_UserPostId",
                table: "UserComments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_UserPosts_UserPostId",
                table: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserLikes_UserPostId",
                table: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserComments_UserPostId",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "UserPostId",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "UserPostId",
                table: "UserComments");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserLikes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserLikes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserComments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_UserComments_PostId",
                table: "UserComments",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserComments_UserPosts_PostId",
                table: "UserComments",
                column: "PostId",
                principalTable: "UserPosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_UserPosts_Id",
                table: "UserLikes",
                column: "Id",
                principalTable: "UserPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserComments_UserPosts_PostId",
                table: "UserComments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_UserPosts_Id",
                table: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserComments_PostId",
                table: "UserComments");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserLikes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserLikes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "UserPostId",
                table: "UserLikes",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserComments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserPostId",
                table: "UserComments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_UserPostId",
                table: "UserLikes",
                column: "UserPostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserComments_UserPostId",
                table: "UserComments",
                column: "UserPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserComments_UserPosts_UserPostId",
                table: "UserComments",
                column: "UserPostId",
                principalTable: "UserPosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_UserPosts_UserPostId",
                table: "UserLikes",
                column: "UserPostId",
                principalTable: "UserPosts",
                principalColumn: "Id");
        }
    }
}
