using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWalker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "user_sessions",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_user_sessions_Token",
                table: "user_sessions",
                newName: "IX_user_sessions_TokenHash");

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "user_sessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "user_sessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Os",
                table: "user_sessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "Os",
                table: "user_sessions");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "user_sessions",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_user_sessions_TokenHash",
                table: "user_sessions",
                newName: "IX_user_sessions_Token");
        }
    }
}
