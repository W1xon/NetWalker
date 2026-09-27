using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWalker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGracePeriodToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreviousTokenExpiresAt",
                table: "user_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PreviousTokenHash",
                table: "user_sessions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousTokenExpiresAt",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "PreviousTokenHash",
                table: "user_sessions");
        }
    }
}
