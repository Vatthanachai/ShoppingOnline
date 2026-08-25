using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingOnline.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusAndOrderItemVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "tb_orderitem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "tb_order",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.CreateIndex(
                name: "IX_tb_orderitem_VendorId",
                table: "tb_orderitem",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_orderitem_tb_vendor_VendorId",
                table: "tb_orderitem",
                column: "VendorId",
                principalTable: "tb_vendor",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_orderitem_tb_vendor_VendorId",
                table: "tb_orderitem");

            migrationBuilder.DropIndex(
                name: "IX_tb_orderitem_VendorId",
                table: "tb_orderitem");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "tb_orderitem");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "tb_order");
        }
    }
}
