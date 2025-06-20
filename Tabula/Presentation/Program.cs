using Domain.Interfaces;
using Legacy.DataAccess.Database;
using Legacy.DataAccess.Extensions;
using Legacy.DataAccess.Services;
using Legacy.Identity.Extensions;
using Legacy.Identity.Endpoints;
using Legacy.Identity.Database;
using Legacy.Identity.Interfaces;
using Tabula.Services.WebApi.Endpoints;

namespace Tabula.Services.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();

        builder.AddNpgsqlDbContext<ShoppingListDbContext>(connectionName: "TabulaDb");
        builder.AddNpgsqlDbContext<IdentityDatabaseContext>(connectionName: "IdentityDb");

        builder.Services.AddRepositories();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        builder.Services.AddScoped<IDatabaseService, DatabaseChecker>();

        // Dodajemy serwisy Identity
        builder.Services.AddScoped<IIdentityDatabaseInitializer, 
            Legacy.Identity.Services.IdentityDatabaseInitializer>();

        builder.Services.AddSwaggerAndJwtHandling(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var shoppingListDatabaseChecker = serviceProvider.GetRequiredService<IDatabaseService>();
            var shoppingListDatabaseInitializer = serviceProvider.GetRequiredService<IDatabaseInitializer>();
            var identityDatabaseInitializer = serviceProvider.GetRequiredService<IIdentityDatabaseInitializer>();

            try
            {
                logger.LogInformation("Checking databases connections...");
                if (await shoppingListDatabaseChecker.CheckConnectionAsync())
                    logger.LogInformation("Database connection is successful");
                else
                    logger.LogError("Database connection failed");

                logger.LogInformation("Initializing database...");
                await shoppingListDatabaseChecker.InitializeAsync();
                logger.LogInformation("Database initialization completed.");

                logger.LogInformation("Initializing Identity...");
                await identityDatabaseInitializer.InitializeAsync();
                logger.LogInformation("Identity initialization completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database initialization");
                throw;
            }
        }

        #region Endpoints

        app.MapGroup("/shoppinglists")
            .WithTags("ShoppingLists")
            .MapShoppingListsEndpoint();

        app.MapGroup("/items")
            .WithTags("Items")
            .MapItemsEndpoint();

        app.MapGroup("/auth")
            .WithTags("Authentication")
            .MapIdentityEndpoint();

        app.MapGroup("/sharing")
            .WithTags("Sharing")
            .MapSharingEndpoint();

        app.MapTagsEndpoints();

        #endregion

        await app.RunAsync();
    }
}