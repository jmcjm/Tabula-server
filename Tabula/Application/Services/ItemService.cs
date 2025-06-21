using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ItemService(IItemRepository itemRepository, ILogger<ItemService> logger)
{
    
}