using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWalker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomChatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    chat_msg = table.Column<string[]>(type: "varchar[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_chat", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_chat");
        }
    }
}
