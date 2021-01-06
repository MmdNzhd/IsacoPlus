using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class addticket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(maxLength: 200, nullable: false),
                    Content = table.Column<string>(maxLength: 500, nullable: false),
                    SenderFile = table.Column<string>(nullable: true),
                    ReceiverFile = table.Column<string>(nullable: true),
                    TicketPriorityStatus = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Answer = table.Column<string>(maxLength: 500, nullable: false),
                    AnswerDate = table.Column<DateTime>(nullable: true),
                    IsReciverSeen = table.Column<bool>(nullable: false),
                    ReceiverSeenDate = table.Column<DateTime>(nullable: true),
                    IsSenderSeen = table.Column<bool>(nullable: false),
                    SenderSeenDate = table.Column<DateTime>(nullable: true),
                    SenderId = table.Column<string>(nullable: true),
                    ReceiverId = table.Column<string>(nullable: true),
                    TicketStatus = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ReceiverId",
                table: "Tickets",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SenderId",
                table: "Tickets",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
