using Domain.Enums;
using Domain.Records;
using ErrorOr;

namespace Domain.Entities;

public class ShareEntity
{
    public ShareId Id { get; private set; }
    public ShoppingListId ShoppingListId { get; private set; }
    public UserId SharedWithUserId { get; private set; }
    public SharePermission Permission { get; private set; }
    public DateTime SharedAt { get; private set; }
    
    private ShareEntity(ShoppingListId shoppingListId, UserId sharedWithUserId, SharePermission permission, DateTime? sharedAt, ShareId? id)
    {
        Id = id ?? new ShareId(Guid.NewGuid());
        ShoppingListId = shoppingListId;
        SharedWithUserId = sharedWithUserId;
        Permission = permission;
        SharedAt = sharedAt ?? DateTime.UtcNow;
    }
    
    public static ErrorOr<ShareEntity> Create(ShoppingListId shoppingListId, UserId sharedWithUserId, SharePermission permission)
    {
        var entity = new ShareEntity(
            shoppingListId: shoppingListId,
            sharedWithUserId: sharedWithUserId,
            permission: permission,
            sharedAt: null,
            id: null);

        return entity;
    }
    
    public static ErrorOr<ShareEntity> Restore(ShareId id, ShoppingListId shoppingListId, UserId sharedWithUserId, SharePermission permission, DateTime sharedAt)
    {
        var entity = new ShareEntity(
            shoppingListId: shoppingListId,
            sharedWithUserId: sharedWithUserId,
            permission: permission,
            sharedAt: sharedAt,
            id: id);

        return entity;
    }
    
    public void UpdatePermission(SharePermission permission)
    {
        Permission = permission;
    }
    
    public bool CheckPermission(SharePermission permission)
    {
        return Permission >= permission;
    }
} 