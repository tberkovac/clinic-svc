using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenamingLeaveRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_LeaveRequest_LeaveRequestId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequest",
                table: "LeaveRequest");

            migrationBuilder.RenameTable(
                name: "LeaveRequest",
                newName: "LeaveRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests",
                column: "LeaveRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_LeaveRequests_LeaveRequestId",
                table: "Users",
                column: "LeaveRequestId",
                principalTable: "LeaveRequests",
                principalColumn: "LeaveRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_LeaveRequests_LeaveRequestId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests");

            migrationBuilder.RenameTable(
                name: "LeaveRequests",
                newName: "LeaveRequest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequest",
                table: "LeaveRequest",
                column: "LeaveRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_LeaveRequest_LeaveRequestId",
                table: "Users",
                column: "LeaveRequestId",
                principalTable: "LeaveRequest",
                principalColumn: "LeaveRequestId");
        }
    }
}
