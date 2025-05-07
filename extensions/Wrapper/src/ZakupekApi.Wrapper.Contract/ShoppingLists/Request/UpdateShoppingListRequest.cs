namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public record UpdateShoppingListRequest(
    string? StoreName,
    DateTime? PlannedShoppingDate,
    string Title
);
