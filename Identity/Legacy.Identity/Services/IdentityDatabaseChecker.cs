using Legacy.Identity.Database;
using Legacy.Identity.Interfaces;
using Microsoft.Extensions.Logging;

namespace Legacy.Identity.Services;

public class IdentityDatabaseChecker(IdentityDatabaseContext context, ILogger<IdentityDatabaseChecker> logger) : IIdentityDatabaseChecker
{
    public async Task<bool> CheckConnectionAsync()
    {
        logger.LogInformation("Checking database connection...");
        return await context.Database.CanConnectAsync();
    }
}
