using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Database.Context.Extensions;

public class ShippingAddressTableConfig : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.ToTable($"tb_{nameof(ShippingAddress).ToLower()}");
        builder.HasKey(k => k.ShippingAddressId);
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
    }
}