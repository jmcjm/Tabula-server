using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<ItemDbModel>
{
    public void Configure(EntityTypeBuilder<ItemDbModel> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .HasColumnName("ProductName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("Quantity")
            .IsRequired();
        
        builder.Property(i => i.Bought)
            .HasColumnName("Bought")
            .IsRequired();

        builder.Property(i => i.ShoppingListId)
            .HasColumnName("ShoppingListId")
            .IsRequired();

        builder.HasOne<ShoppingListDbModel>()
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}