using FastEndpoints;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public record UpdateShoppingListEndpointRequest(
    [property: BindFrom("id")] int Id,
    [property: FromBody] UpdateShoppingListRequest Body
);
