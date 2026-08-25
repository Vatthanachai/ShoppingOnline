using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Database.Context.Extensions;

public class OrderItemAllocationTableConfig : IEntityTypeConfiguration<OrderItemAllocation>
{
    public void Configure(EntityTypeBuilder<OrderItemAllocation> builder)
    {
        builder.ToTable($"tb_{nameof(OrderItemAllocation).ToLower()}");
        builder.HasKey(k => k.OrderItemAllocationId);

        builder.HasOne(a => a.OrderItem)
            .WithMany(i => i.Allocations)
            .HasForeignKey(a => a.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Stock)
            .WithMany()
            .HasForeignKey(a => a.StockId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Vendor)
            .WithMany()
            .HasForeignKey(a => a.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
