using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tabula.Infrastructure.DataAccess.Database;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Infrastructure.DataAccess.Services;

public class DatabaseInitializer(ShoppingListDbContext context, ILogger<DatabaseInitializer> logger)
    : IDatabaseInitializer
{
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
    
}
