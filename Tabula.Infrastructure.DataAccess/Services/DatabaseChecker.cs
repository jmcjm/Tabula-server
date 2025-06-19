using Microsoft.Extensions.Logging;
using Tabula.Infrastructure.DataAccess.Database;
using Tabula.Infrastructure.DataAccess.Interfaces;

namespace Tabula.Infrastructure.DataAccess.Services;

public class DatabaseChecker(ShoppingListDbContext context, ILogger<DatabaseChecker> logger) : IDatabaseChecker
{
    public async Task<bool> CheckConnectionAsync()
    {
        logger.LogInformation("Checking database connection...");
        return await context.Database.CanConnectAsync();
    }
}
