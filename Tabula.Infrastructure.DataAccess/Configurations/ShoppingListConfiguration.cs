using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Configurations;

public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingListDao>
{
    public void Configure(EntityTypeBuilder<ShoppingListDao> builder)
    {
        builder.ToTable("ShoppingLists");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasColumnName("UserId")
            .IsRequired();
    }
}