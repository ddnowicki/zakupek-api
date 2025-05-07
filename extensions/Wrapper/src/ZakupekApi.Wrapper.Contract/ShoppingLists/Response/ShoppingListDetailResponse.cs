namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

public record ProductInListResponse(
    int Id,
    string Name,
    int Quantity,
    int StatusId,
    string Status,
    DateTime CreatedAt
);

public record ShoppingListDetailResponse(
    int Id,
    string? Title,
    string? StoreName,
    DateTime? PlannedShoppingDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Source,
    string? ShopName,
    IEnumerable<ProductInListResponse> Products
);
