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

        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_Tags_UserId");
        
        builder.HasIndex(t => new { t.UserId, t.Name })
            .IsUnique()
            .HasDatabaseName("IX_Tags_UserId_Name_Unique");
        
        builder.HasIndex(t => new { t.Id, t.UserId })
            .IsUnique()
            .HasDatabaseName("IX_Tags_Id_UserId_Unique");
    }
}