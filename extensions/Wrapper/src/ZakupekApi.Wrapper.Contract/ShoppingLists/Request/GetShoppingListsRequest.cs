using FastEndpoints;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public record GetShoppingListsRequest(
    [property: QueryParam] int Page = 1,
    [property: QueryParam] int PageSize = 10,
    [property: QueryParam] string Sort = "newest"
);
