using Legacy.DataAccess.Interfaces;
using Legacy.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Legacy.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dodaje wszystkie repozytoria do DI
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IShoppingListShareRepository, ShoppingListShareRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        return services;
    }
}