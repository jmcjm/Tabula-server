namespace Domain.Enums;

public enum SharePermission
{
    // Can only read the shopping list and see the items
    Read = 1,
    
    // Can read and modify the shopping list and items
    Modify = 2,
    
    // Can read, modify and delete the shopping list and items
    Admin = 3
} 