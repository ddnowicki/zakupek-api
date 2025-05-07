namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public record GenerateShoppingListRequest(
    string? Title,
    DateTime? PlannedShoppingDate,
    string? StoreName
);