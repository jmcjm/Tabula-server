using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<TagDao>
{
    public void Configure(EntityTypeBuilder<TagDao> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Color)
            .HasColumnName("Color")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        // Index na UserId dla wydajności
        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_Tags_UserId");

        // Unique constraint na Name per User
        builder.HasIndex(t => new { t.UserId, t.Name })
            .IsUnique()
            .HasDatabaseName("IX_Tags_UserId_Name_Unique");

        // Konfiguracja relacji many-to-many z ShoppingList
        builder.HasMany(t => t.ShoppingLists)
            .WithMany(s => s.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "ShoppingListTags",
                j => j.HasOne<ShoppingListDao>()
                    .WithMany()
                    .HasForeignKey("ShoppingListId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<TagDao>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("ShoppingListId", "TagId");
                    j.ToTable("ShoppingListTags");
                });
    }
} 