using Microsoft.EntityFrameworkCore;

using ShoppingOnline.Component.Data.Context;
using ShoppingOnline.Database.Context.Extensions;

namespace ShoppingOnline.Database.Context;

public class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : BaseDbContext(options), IShoppingDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderTableConfig());
        modelBuilder.ApplyConfiguration(new OrderItemTableConfig());
        modelBuilder.ApplyConfiguration(new OrderItemAllocationTableConfig());

        modelBuilder.ApplyConfiguration(new ProductTableConfig());
        modelBuilder.ApplyConfiguration(new ProductCategoryTableConfig());

        modelBuilder.ApplyConfiguration(new PurchaseOrderTableConfig());
        modelBuilder.ApplyConfiguration(new PurchaseOrderItemTableConfig());

        modelBuilder.ApplyConfiguration(new ShippingAddressTableConfig());
        modelBuilder.ApplyConfiguration(new StockTableConfig());

        modelBuilder.ApplyConfiguration(new UserTableConfig());
        modelBuilder.ApplyConfiguration(new VendorTableConfig());

        base.OnModelCreating(modelBuilder);
    }
}