using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReceivingRequests_UserRequests_UserRequestId",
                table: "UserReceivingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReceivingRequests_Users_TargetUserId",
                table: "UserReceivingRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserReceivingRequests_TargetUserId",
                table: "UserReceivingRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserReceivingRequests_UserRequestId",
                table: "UserReceivingRequests");

            migrationBuilder.DropColumn(
                name: "UserRequestId",
                table: "UserReceivingRequests");

            migrationBuilder.AlterColumn<int>(
                name: "UserReceivingRequestId",
                table: "UserRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserReceivingRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRequests_UserReceivingRequestId",
                table: "UserRequests",
                column: "UserReceivingRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReceivingRequests_UserId",
                table: "UserReceivingRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReceivingRequests_Users_UserId",
                table: "UserReceivingRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRequests_UserReceivingRequests_UserReceivingRequestId",
                table: "UserRequests",
                column: "UserReceivingRequestId",
                principalTable: "UserReceivingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReceivingRequests_Users_UserId",
                table: "UserReceivingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRequests_UserReceivingRequests_UserReceivingRequestId",
                table: "UserRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserRequests_UserReceivingRequestId",
                table: "UserRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserReceivingRequests_UserId",
                table: "UserReceivingRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserReceivingRequests");

            migrationBuilder.AlterColumn<int>(
                name: "UserReceivingRequestId",
                table: "UserRequests",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "UserRequestId",
                table: "UserReceivingRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserReceivingRequests_TargetUserId",
                table: "UserReceivingRequests",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReceivingRequests_UserRequestId",
                table: "UserReceivingRequests",
                column: "UserRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReceivingRequests_UserRequests_UserRequestId",
                table: "UserReceivingRequests",
                column: "UserRequestId",
                principalTable: "UserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReceivingRequests_Users_TargetUserId",
                table: "UserReceivingRequests",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
