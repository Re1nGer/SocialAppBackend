using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    public partial class likeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_UserPosts_Id",
                table: "UserLikes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserLikes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_PostId",
                table: "UserLikes",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_UserPosts_PostId",
                table: "UserLikes",
                column: "PostId",
                principalTable: "UserPosts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_UserPosts_PostId",
                table: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserLikes_PostId",
                table: "UserLikes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserLikes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_UserPosts_Id",
                table: "UserLikes",
                column: "Id",
                principalTable: "UserPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
