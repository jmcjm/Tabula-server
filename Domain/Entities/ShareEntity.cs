using Domain.Enums;
using Domain.Records;

namespace Domain.Entities;

public class ShareEntity
{
    public ShareId Id { get; private set; }
    public ShoppingListId ShoppingListId { get; private set; }
    public UserId SharedWithUserId { get; private set; }
    public SharePermission Permission { get; private set; }
    public DateTime SharedAt { get; private set; }
    
    public ShareEntity(ShoppingListId shoppingListId, UserId sharedWithUserId, SharePermission permission, DateTime sharedAt, ShareId? id)
    {
        Id = id ?? new ShareId(Guid.NewGuid());
        ShoppingListId = shoppingListId;
        SharedWithUserId = sharedWithUserId;
        Permission = permission;
        SharedAt = sharedAt;
    }
    
    public void UpdatePermission(SharePermission permission)
    {
        Permission = permission;
    }
} 