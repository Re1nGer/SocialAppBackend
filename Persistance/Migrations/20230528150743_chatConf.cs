using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class chatConf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserChatId",
                table: "UserMessages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserChats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserChatId",
                table: "UserMessages",
                column: "UserChatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChats_UserId",
                table: "UserChats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_UserChats_UserChatId",
                table: "UserMessages",
                column: "UserChatId",
                principalTable: "UserChats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_UserChats_UserChatId",
                table: "UserMessages");

            migrationBuilder.DropTable(
                name: "UserChats");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_UserChatId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "UserChatId",
                table: "UserMessages");
        }
    }
}
