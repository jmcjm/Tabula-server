using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ShareConfiguration : IEntityTypeConfiguration<ShareDbModel>
{
    public void Configure(EntityTypeBuilder<ShareDbModel> builder)
    {
        builder.ToTable("Shares");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SharedWithUserId)
            .HasColumnName("SharedWithUserId")
            .IsRequired();

        builder.Property(s => s.Permission)
            .HasColumnName("Permission")
            .IsRequired();

        builder.Property(s => s.SharedAt)
            .HasColumnName("SharedAt")
            .IsRequired();
        
        builder.HasOne(s => s.ShoppingList)
            .WithMany()
            .HasForeignKey(s => s.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(s => new { s.ShoppingListId, s.SharedWithUserId })
            .IsUnique()
            .HasDatabaseName("IX_ShoppingListShares_ShoppingListId_SharedWithUserId");
    }
} 