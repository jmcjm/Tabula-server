using Domain.Interfaces;
using Legacy.Identity.Interfaces;

namespace Presentation;

public static class DatabaseInitializer
{
    public static async Task EnsureDatabasesInitializedAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var shoppingListDatabaseChecker = scopedServices.GetRequiredService<IDatabaseService>();
        var identityDatabaseInitializer = scopedServices.GetRequiredService<IIdentityDatabaseInitializer>();
        var identityDatabaseChecker = scopedServices.GetRequiredService<IIdentityDatabaseChecker>();

        try
        {
            logger.LogInformation("Checking databases connections...");

            var isTabulaDbConnected = await shoppingListDatabaseChecker.CheckConnectionAsync();
            var isIdentityDbConnected = await identityDatabaseChecker.CheckConnectionAsync();

            if (!isTabulaDbConnected)
                throw new Exception("Could not connect Tabula database. Check connection settings.");
            if (!isIdentityDbConnected)
                throw new Exception("Could not connect Identity database. Check connection settings.");

            logger.LogInformation("Successfully connected to databases.");
            logger.LogInformation("Initializing databases...");

            await shoppingListDatabaseChecker.InitializeAsync();
            await identityDatabaseInitializer.InitializeAsync();

            logger.LogInformation("Databases initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
    }
}