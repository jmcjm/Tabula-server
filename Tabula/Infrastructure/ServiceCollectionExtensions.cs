using Domain.Interfaces;
using Infrastructure.EfRepositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IShareRepository, ShareRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        return services;
    }
}