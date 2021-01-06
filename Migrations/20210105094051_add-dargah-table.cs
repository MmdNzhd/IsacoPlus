using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class adddargahtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "ShoppingCarts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GetPayments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    FinallyAmountWithTax = table.Column<decimal>(nullable: false),
                    IsSucceed = table.Column<bool>(nullable: false),
                    InvoiceKey = table.Column<string>(nullable: true),
                    TransactionCode = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    TrackingNumber = table.Column<string>(nullable: true),
                    ErrorDescription = table.Column<string>(nullable: true),
                    ErrorCode = table.Column<string>(nullable: true),
                    IsImmeditely = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GetPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GetPayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionCode = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    FinallyAmountWithTax = table.Column<decimal>(nullable: false),
                    Information = table.Column<string>(nullable: true),
                    IsSucceed = table.Column<bool>(nullable: false),
                    PaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_GetPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "GetPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_ShoppingCarts_PaymentId",
            //    table: "ShoppingCarts",
            //    column: "PaymentId",
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GetPayments_UserId",
                table: "GetPayments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions",
                column: "PaymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_GetPayments_PaymentId",
                table: "ShoppingCarts",
                column: "PaymentId",
                principalTable: "GetPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_GetPayments_PaymentId",
                table: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "GetPayments");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_PaymentId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "ShoppingCarts");
        }
    }
}
