using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.ShoppingLists.Endpoints;


public class GetShoppingListByIdEndpoint : Endpoint<GetShoppingListByIdRequest, ErrorOr<ShoppingListDetailResponse>>
{
    public IShoppingListService ShoppingListService { get; set; } = default!;

    public override void Configure()
    {
        Get("/api/shoppinglists/{id}");
    }

    public override async Task<ErrorOr<ShoppingListDetailResponse>> ExecuteAsync(GetShoppingListByIdRequest req, CancellationToken ct)
        => await ShoppingListService.GetShoppingListByIdAsync(req.Id, User.GetUserId());
}
