using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWalker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoomPlayersMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionCode = table.Column<string>(type: "text", nullable: false),
                    TicketCode = table.Column<string>(type: "text", nullable: true),
                    IsTicketClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    player_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rooms_SessionCode",
                table: "rooms",
                column: "SessionCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}
