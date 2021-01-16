using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class editcartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discount",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CartItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CartItems");
        }
    }
}
