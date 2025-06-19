using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingListDbModel>
{
    public void Configure(EntityTypeBuilder<ShoppingListDbModel> builder)
    {
        builder.ToTable("ShoppingLists");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.HasIndex(s => s.Name);
    }
}