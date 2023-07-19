using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class BookmarksUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostBookmark_UserPosts_UserPostId",
                table: "PostBookmark");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostBookmark",
                table: "PostBookmark");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "PostBookmark",
                newName: "PostBookmarks");

            migrationBuilder.RenameIndex(
                name: "IX_PostBookmark_UserPostId",
                table: "PostBookmarks",
                newName: "IX_PostBookmarks_UserPostId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PostBookmarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostBookmarks",
                table: "PostBookmarks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostBookmarks_UserId",
                table: "PostBookmarks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostBookmarks_UserPosts_UserPostId",
                table: "PostBookmarks",
                column: "UserPostId",
                principalTable: "UserPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostBookmarks_Users_UserId",
                table: "PostBookmarks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostBookmarks_UserPosts_UserPostId",
                table: "PostBookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_PostBookmarks_Users_UserId",
                table: "PostBookmarks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostBookmarks",
                table: "PostBookmarks");

            migrationBuilder.DropIndex(
                name: "IX_PostBookmarks_UserId",
                table: "PostBookmarks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PostBookmarks");

            migrationBuilder.RenameTable(
                name: "PostBookmarks",
                newName: "PostBookmark");

            migrationBuilder.RenameIndex(
                name: "IX_PostBookmarks_UserPostId",
                table: "PostBookmark",
                newName: "IX_PostBookmark_UserPostId");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostBookmark",
                table: "PostBookmark",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostBookmark_UserPosts_UserPostId",
                table: "PostBookmark",
                column: "UserPostId",
                principalTable: "UserPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
