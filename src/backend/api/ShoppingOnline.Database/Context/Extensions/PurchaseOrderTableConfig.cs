using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Database.Context.Extensions;

public class PurchaseOrderTableConfig : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable($"tb_{nameof(PurchaseOrder).ToLower()}");
        builder.HasKey(k => k.PurchaseOrderId);
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.CreatedOn)
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);
        builder.Property(p => p.ModifiedDate)
            .HasColumnType("timestamp with time zone");
        builder.Property(p => p.SentOn)
            .HasColumnType("timestamp with time zone");
    }
}
