using Domain.Interfaces;
using Infrastructure;
using Legacy.Identity.Database;
using Legacy.Identity.Endpoints;
using Legacy.Identity.Extensions;
using Legacy.Identity.Interfaces;
using Legacy.Identity.Services;
using Presentation.Endpoints;
using Tabula.Services.WebApi.Endpoints;

namespace Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();
        
        builder.Services.AddProblemDetails();

        builder.AddNpgsqlDbContext<TabulaDbContext>(connectionName: "TabulaDb");
        builder.AddNpgsqlDbContext<IdentityDatabaseContext>(connectionName: "IdentityDb");

        builder.Services.AddInfrastructure();
        
        // Dodajemy serwisy Identity
        builder.Services.AddScoped<IIdentityDatabaseInitializer, IdentityDatabaseInitializer>();
        builder.Services.AddScoped<IIdentityDatabaseChecker, IdentityDatabaseChecker>();
        
        builder.Services.AddSwaggerAndJwtHandling(builder.Configuration);

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

        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        await DatabaseInitializer.EnsureDatabasesInitializedAsync(app.Services, logger);

        app.MapGroup("/shoppinglists")
            .WithTags("ShoppingLists")
            .MapShoppingListsEndpoint();

        app.MapItemsEndpoint();

        app.MapGroup("/auth")
            .WithTags("Authentication")
            .MapIdentityEndpoint();

        app.MapGroup("/sharing")
            .WithTags("Sharing")
            .MapSharingEndpoint();

        app.MapTagsEndpoints();

        await app.RunAsync();

}