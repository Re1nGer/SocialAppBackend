using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class removeUserReceivingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserReceivingRequests",
                table: "UserReceivingRequests");

            migrationBuilder.RenameTable(
                name: "UserReceivingRequests",
                newName: "UserReceivingRequest");

            migrationBuilder.RenameIndex(
                name: "IX_UserReceivingRequests_UserId",
                table: "UserReceivingRequest",
                newName: "IX_UserReceivingRequest_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserReceivingRequest",
                table: "UserReceivingRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReceivingRequest_Users_UserId",
                table: "UserReceivingRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReceivingRequest_Users_UserId",
                table: "UserReceivingRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserReceivingRequest",
                table: "UserReceivingRequest");

            migrationBuilder.RenameTable(
                name: "UserReceivingRequest",
                newName: "UserReceivingRequests");

            migrationBuilder.RenameIndex(
                name: "IX_UserReceivingRequest_UserId",
                table: "UserReceivingRequests",
                newName: "IX_UserReceivingRequests_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserReceivingRequests",
                table: "UserReceivingRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRequests_UserReceivingRequestId",
                table: "UserRequests",
                column: "UserReceivingRequestId",
                unique: true);

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
    }
}
