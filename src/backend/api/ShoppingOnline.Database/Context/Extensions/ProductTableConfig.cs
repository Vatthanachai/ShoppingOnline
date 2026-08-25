using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Database.Context.Extensions;

public class ProductTableConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable($"tb_{nameof(Product).ToLower()}");

        builder.HasKey(k => k.ProductId);

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