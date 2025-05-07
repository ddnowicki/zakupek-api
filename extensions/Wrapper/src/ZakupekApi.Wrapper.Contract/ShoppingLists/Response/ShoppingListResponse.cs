namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

public record ShoppingListResponse(
    int Id,
    string? Title,
    int ProductsCount,
    DateTime? PlannedShoppingDate,
    DateTime CreatedAt,
    string Source,
    string? StoreName
);
