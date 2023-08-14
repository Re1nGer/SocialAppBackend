using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    public partial class UserChatModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_Users_FollowerUserId",
                table: "UserFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_Users_FollowingId",
                table: "UserFollowers");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "UserReceivingRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollowers",
                table: "UserFollowers");

            migrationBuilder.RenameTable(
                name: "UserFollowers",
                newName: "UserFollower");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowers_FollowingId",
                table: "UserFollower",
                newName: "IX_UserFollower_FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowers_FollowerUserId",
                table: "UserFollower",
                newName: "IX_UserFollower_FollowerUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserWithChatId",
                table: "UserChats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollower",
                table: "UserFollower",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollower_Users_FollowerUserId",
                table: "UserFollower",
                column: "FollowerUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollower_Users_FollowingId",
                table: "UserFollower",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollower_Users_FollowerUserId",
                table: "UserFollower");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollower_Users_FollowingId",
                table: "UserFollower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollower",
                table: "UserFollower");

            migrationBuilder.DropColumn(
                name: "UserWithChatId",
                table: "UserChats");

            migrationBuilder.RenameTable(
                name: "UserFollower",
                newName: "UserFollowers");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollower_FollowingId",
                table: "UserFollowers",
                newName: "IX_UserFollowers_FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollower_FollowerUserId",
                table: "UserFollowers",
                newName: "IX_UserFollowers_FollowerUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollowers",
                table: "UserFollowers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessages_UserChats_UserChatId",
                        column: x => x.UserChatId,
                        principalTable: "UserChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMessages_Users_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMessages_Users_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReceivingRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReceivingRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReceivingRequest_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_SourceId",
                table: "UserMessages",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_TargetId",
                table: "UserMessages",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserChatId",
                table: "UserMessages",
                column: "UserChatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReceivingRequest_UserId",
                table: "UserReceivingRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_Users_FollowerUserId",
                table: "UserFollowers",
                column: "FollowerUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_Users_FollowingId",
                table: "UserFollowers",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
