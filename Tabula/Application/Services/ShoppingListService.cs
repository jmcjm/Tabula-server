using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ShoppingListService(IShoppingListRepository shoppingListRepository, ILogger<ShoppingListService> logger)
{
    
}