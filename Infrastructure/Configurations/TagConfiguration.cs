using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<TagDbModel>
{
    public void Configure(EntityTypeBuilder<TagDbModel> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Color)
            .HasColumnName("Color")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasColumnName("UserId")
            .IsRequired();
        
        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_Tags_UserId");
        
        builder.HasIndex(t => new { t.UserId, t.Name })
            .IsUnique()
            .HasDatabaseName("IX_Tags_UserId_Name_Unique");
        
        builder.HasMany(t => t.ShoppingLists)
            .WithMany(s => s.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "ShoppingListTags",
                j => j.HasOne<ShoppingListDbModel>()
                    .WithMany()
                    .HasForeignKey("ShoppingListId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<TagDbModel>()
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