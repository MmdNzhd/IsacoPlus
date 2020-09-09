using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class Edit_Factor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factors_Clients_ClientId",
                table: "Factors");

            migrationBuilder.DropIndex(
                name: "IX_Factors_ClientId",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Factors");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Factors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Factors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factors_UserId",
                table: "Factors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Factors_AspNetUsers_UserId",
                table: "Factors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factors_AspNetUsers_UserId",
                table: "Factors");

            migrationBuilder.DropIndex(
                name: "IX_Factors_UserId",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Factors");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Factors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Factors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factors_ClientId",
                table: "Factors",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Factors_Clients_ClientId",
                table: "Factors",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
