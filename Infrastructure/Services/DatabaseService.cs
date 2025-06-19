using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class DatabaseService(TabulaDbContext context, ILogger<DatabaseService> logger) : IDatabaseService
{
    /// <summary>
    /// Checks if the database connection is successful.
    /// </summary>
    public async Task<bool> CheckConnectionAsync()
    {
        logger.LogInformation("Checking database connection...");
        return await context.Database.CanConnectAsync();
    }

    /// <summary>
    /// Checks migrations and applies them if necessary.
    /// </summary>
    /// <exception cref="Exception">If an error occurs while applying migrations.</exception>
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