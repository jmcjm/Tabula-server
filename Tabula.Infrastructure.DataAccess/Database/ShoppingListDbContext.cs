using Microsoft.EntityFrameworkCore;
using Tabula.Infrastructure.DataAccess.Configurations;
using Tabula.Infrastructure.DataAccess.Models;

namespace Tabula.Infrastructure.DataAccess.Database;

public class ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options) : DbContext(options)
{
    public DbSet<ShoppingListDao> ShoppingLists { get; set; }
    public DbSet<ItemDao> Items { get; set; }
    public DbSet<ShoppingListShareDao> ShoppingListShares { get; set; }
    public DbSet<TagDao> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new ShoppingListConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());
        modelBuilder.ApplyConfiguration(new ShoppingListShareConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
    }
}