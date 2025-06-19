using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Configurations;

public class ShoppingListShareConfiguration : IEntityTypeConfiguration<ShoppingListShareDao>
{
    public void Configure(EntityTypeBuilder<ShoppingListShareDao> builder)
    {
        builder.ToTable("ShoppingListShares");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.OwnerId)
            .HasColumnName("OwnerId")
            .IsRequired();

        builder.Property(s => s.SharedWithUserId)
            .HasColumnName("SharedWithUserId")
            .IsRequired();

        builder.Property(s => s.Permission)
            .HasColumnName("Permission")
            .HasConversion<int>() // Przechowuj enum jako int
            .IsRequired();

        builder.Property(s => s.SharedAt)
            .HasColumnName("SharedAt")
            .IsRequired();

        // Relacja z ShoppingList
        builder.HasOne(s => s.ShoppingList)
            .WithMany() // ShoppingList może być udostępniona wielu użytkownikom
            .HasForeignKey(s => s.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unikalny index - jeden użytkownik może mieć tylko jedno udostępnienie dla danej listy
        builder.HasIndex(s => new { s.ShoppingListId, s.SharedWithUserId })
            .IsUnique()
            .HasDatabaseName("IX_ShoppingListShares_ShoppingListId_SharedWithUserId");
    }
} 