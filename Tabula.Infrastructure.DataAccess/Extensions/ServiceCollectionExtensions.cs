using Microsoft.Extensions.DependencyInjection;
using Tabula.Infrastructure.DataAccess.Interfaces;
using Tabula.Infrastructure.DataAccess.Repositories;

namespace Tabula.Infrastructure.DataAccess.Extensions;

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