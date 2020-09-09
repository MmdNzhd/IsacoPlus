using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class AddFavoriteAndEditComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Comments",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
