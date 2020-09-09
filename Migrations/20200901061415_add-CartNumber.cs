using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class addCartNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartNumber",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartNumber",
                table: "AspNetUsers");
        }
    }
}
