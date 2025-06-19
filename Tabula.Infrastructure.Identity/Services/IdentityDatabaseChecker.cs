using Microsoft.Extensions.Logging;
using Tabula.Infrastructure.Identity.Database;
using Tabula.Infrastructure.Identity.Interfaces;

namespace Tabula.Infrastructure.Identity.Services;

public class IdentityDatabaseChecker(IdentityDatabaseContext context, ILogger<IdentityDatabaseChecker> logger) : IIdentityDatabaseChecker
{
    public async Task<bool> CheckConnectionAsync()
    {
        logger.LogInformation("Checking database connection...");
        return await context.Database.CanConnectAsync();
    }
}
