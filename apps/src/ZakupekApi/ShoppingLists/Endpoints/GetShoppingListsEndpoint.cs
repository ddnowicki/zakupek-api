using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.ShoppingLists.Endpoints;

public class GetShoppingListsEndpoint : Endpoint<GetShoppingListsRequest, ErrorOr<ShoppingListsResponse>>
{
    public IShoppingListService ShoppingListService { get; set; } = default!;

    public override void Configure()
    {
        Get("/api/shoppinglists");
    }

    public override async Task<ErrorOr<ShoppingListsResponse>> ExecuteAsync(GetShoppingListsRequest req, CancellationToken ct)
        => await ShoppingListService.GetShoppingListsAsync(User.GetUserId(), req.Page, req.PageSize, req.Sort);
}
