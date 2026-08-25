using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShoppingOnline.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesFlowRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "tb_stock",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "tb_orderitem",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderItemId",
                table: "tb_stock",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "tb_product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRatePercent",
                table: "tb_product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRatePercent",
                table: "tb_orderitem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressLine1",
                table: "tb_order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressLine2",
                table: "tb_order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingCity",
                table: "tb_order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingCountry",
                table: "tb_order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingPostalCode",
                table: "tb_order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingState",
                table: "tb_order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "tb_orderitemallocation",
                columns: table => new
                {
                    OrderItemAllocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemId = table.Column<int>(type: "integer", nullable: false),
                    StockId = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_orderitemallocation", x => x.OrderItemAllocationId);
                    table.ForeignKey(
                        name: "FK_tb_orderitemallocation_tb_orderitem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "tb_orderitem",
                        principalColumn: "OrderItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_orderitemallocation_tb_stock_StockId",
                        column: x => x.StockId,
                        principalTable: "tb_stock",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_orderitemallocation_tb_vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "tb_vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_purchaseorder",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SentOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_purchaseorder", x => x.PurchaseOrderId);
                    table.ForeignKey(
                        name: "FK_tb_purchaseorder_tb_vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "tb_vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_purchaseorderitem",
                columns: table => new
                {
                    PurchaseOrderItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    QuantityOrdered = table.Column<int>(type: "integer", nullable: false),
                    QuantityReceived = table.Column<int>(type: "integer", nullable: false),
                    UnitCostQuoted = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_purchaseorderitem", x => x.PurchaseOrderItemId);
                    table.ForeignKey(
                        name: "FK_tb_purchaseorderitem_tb_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tb_product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_purchaseorderitem_tb_purchaseorder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "tb_purchaseorder",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_stock_PurchaseOrderItemId",
                table: "tb_stock",
                column: "PurchaseOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_orderitemallocation_OrderItemId",
                table: "tb_orderitemallocation",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_orderitemallocation_StockId",
                table: "tb_orderitemallocation",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_orderitemallocation_VendorId",
                table: "tb_orderitemallocation",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_purchaseorder_VendorId",
                table: "tb_purchaseorder",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_purchaseorderitem_ProductId",
                table: "tb_purchaseorderitem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_purchaseorderitem_PurchaseOrderId",
                table: "tb_purchaseorderitem",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_stock_tb_purchaseorderitem_PurchaseOrderItemId",
                table: "tb_stock",
                column: "PurchaseOrderItemId",
                principalTable: "tb_purchaseorderitem",
                principalColumn: "PurchaseOrderItemId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_stock_tb_purchaseorderitem_PurchaseOrderItemId",
                table: "tb_stock");

            migrationBuilder.DropTable(
                name: "tb_orderitemallocation");

            migrationBuilder.DropTable(
                name: "tb_purchaseorderitem");

            migrationBuilder.DropTable(
                name: "tb_purchaseorder");

            migrationBuilder.DropIndex(
                name: "IX_tb_stock_PurchaseOrderItemId",
                table: "tb_stock");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderItemId",
                table: "tb_stock");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "tb_product");

            migrationBuilder.DropColumn(
                name: "TaxRatePercent",
                table: "tb_product");

            migrationBuilder.DropColumn(
                name: "TaxRatePercent",
                table: "tb_orderitem");

            migrationBuilder.DropColumn(
                name: "ShippingAddressLine1",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "ShippingAddressLine2",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "ShippingCity",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "ShippingCountry",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "ShippingPostalCode",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "ShippingState",
                table: "tb_order");

            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "tb_stock",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "tb_orderitem",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "tb_orderitem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
    }
}
