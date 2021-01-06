using Microsoft.EntityFrameworkCore.Migrations;

namespace KaraYadak.Migrations
{
    public partial class adddargahtable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GetPayments_AspNetUsers_UserId",
                table: "GetPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_GetPayments_PaymentId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_GetPayments_PaymentId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GetPayments",
                table: "GetPayments");

            migrationBuilder.RenameTable(
                name: "GetPayments",
                newName: "Payments");

            migrationBuilder.RenameIndex(
                name: "IX_GetPayments_UserId",
                table: "Payments",
                newName: "IX_Payments_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "ShoppingCarts",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_PaymentId",
                table: "ShoppingCarts",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Payments_PaymentId",
                table: "ShoppingCarts",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Payments_PaymentId",
                table: "Transactions",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_UserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Payments_PaymentId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Payments_PaymentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_PaymentId",
                table: "ShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "GetPayments");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_UserId",
                table: "GetPayments",
                newName: "IX_GetPayments_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "ShoppingCarts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GetPayments",
                table: "GetPayments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_PaymentId",
                table: "ShoppingCarts",
                column: "PaymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GetPayments_AspNetUsers_UserId",
                table: "GetPayments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_GetPayments_PaymentId",
                table: "ShoppingCarts",
                column: "PaymentId",
                principalTable: "GetPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_GetPayments_PaymentId",
                table: "Transactions",
                column: "PaymentId",
                principalTable: "GetPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
