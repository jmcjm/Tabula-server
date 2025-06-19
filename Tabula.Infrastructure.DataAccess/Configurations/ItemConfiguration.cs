using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<ItemDao>
{
    public void Configure(EntityTypeBuilder<ItemDao> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .HasColumnName("ProductName")
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("Quantity")
            .IsRequired();

        builder.Property(i => i.ShoppingListId)
            .HasColumnName("ShoppingListId")
            .IsRequired();

        builder.HasOne<ShoppingListDao>()
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}