using System.ComponentModel.DataAnnotations;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public record ProductRequest(
    [Required(ErrorMessage = "Product name is required")]
    string Name,
    
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    int Quantity
);

public record CreateShoppingListRequest(
    string? Title,
    IEnumerable<ProductRequest>? Products,
    DateTime? PlannedShoppingDate,
    string? StoreName
);
