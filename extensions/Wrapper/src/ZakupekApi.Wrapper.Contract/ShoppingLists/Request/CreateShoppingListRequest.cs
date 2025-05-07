namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public record ProductRequest(string Name, int Quantity);

public record CreateShoppingListRequest(
    string? Title,
    IEnumerable<ProductRequest>? Products,
    DateTime? PlannedShoppingDate,
    string? StoreName
);
