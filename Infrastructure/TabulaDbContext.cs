using Infrastructure.Configurations;
using Infrastructure.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TabulaDbContext(DbContextOptions<TabulaDbContext> options) : DbContext(options)
{
    public DbSet<ShoppingListDbModel> ShoppingLists { get; set; }
    public DbSet<ItemDbModel> Items { get; set; }
    public DbSet<ShareDbModel> Shares { get; set; }
    public DbSet<TagDbModel> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new ShoppingListConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());
        modelBuilder.ApplyConfiguration(new ShareConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
    }
}