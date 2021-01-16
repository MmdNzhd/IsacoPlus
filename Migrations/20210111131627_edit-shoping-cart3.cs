using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class editshopingcart3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostTrackingNumber",
                table: "ShoppingCarts",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostTrackingNumber",
                table: "Payments",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostTrackingNumber",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "PostTrackingNumber",
                table: "Payments");
        }
    }
}
