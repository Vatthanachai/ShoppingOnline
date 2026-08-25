using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Database.Context.Extensions;

public class PurchaseOrderItemTableConfig : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable($"tb_{nameof(PurchaseOrderItem).ToLower()}");
        builder.HasKey(k => k.PurchaseOrderItemId);

        builder.HasOne(i => i.PurchaseOrder)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
