using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.Database.Context.Extensions;

public class ProductCategoryTableConfig : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable($"tb_{nameof(ProductCategory).ToLower()}");
        builder.HasKey(k => k.ProductCategoryId);
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