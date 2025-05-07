using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.ShoppingLists.Endpoints;

public class GenerateShoppingListEndpoint : Endpoint<GenerateShoppingListRequest, ErrorOr<ShoppingListDetailResponse>>
{
    public IShoppingListService ShoppingListService { get; set; } = default!;

    public override void Configure()
    {
        Post("/api/shoppinglists/generate");
    }

    public override async Task<ErrorOr<ShoppingListDetailResponse>> ExecuteAsync(GenerateShoppingListRequest req, CancellationToken ct)
        => await ShoppingListService.GenerateShoppingListAsync(User.GetUserId(), req);
}