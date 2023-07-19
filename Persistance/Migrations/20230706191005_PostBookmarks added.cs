using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    public partial class PostBookmarksadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentIds",
                table: "UserPosts");

            migrationBuilder.DropColumn(
                name: "LikeIds",
                table: "UserPosts");

            migrationBuilder.CreateTable(
                name: "PostBookmark",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserPostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostBookmark", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostBookmark_UserPosts_UserPostId",
                        column: x => x.UserPostId,
                        principalTable: "UserPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserComments_UserId",
                table: "UserComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostBookmark_UserPostId",
                table: "PostBookmark",
                column: "UserPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserComments_Users_UserId",
                table: "UserComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserComments_Users_UserId",
                table: "UserComments");

            migrationBuilder.DropTable(
                name: "PostBookmark");

            migrationBuilder.DropIndex(
                name: "IX_UserComments_UserId",
                table: "UserComments");

            migrationBuilder.AddColumn<List<int>>(
                name: "CommentIds",
                table: "UserPosts",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "LikeIds",
                table: "UserPosts",
                type: "integer[]",
                nullable: true);
        }
    }
}
