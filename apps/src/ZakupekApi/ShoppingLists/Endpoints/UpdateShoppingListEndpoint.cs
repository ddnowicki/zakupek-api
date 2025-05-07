using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.ShoppingLists.Endpoints;

public class UpdateShoppingListEndpoint : Endpoint<UpdateShoppingListEndpointRequest, ErrorOr<ShoppingListDetailResponse>>
{
    public IShoppingListService ShoppingListService { get; set; } = default!;

    public override void Configure()
    {
        Put("/api/shoppinglists/{id}");
    }

    public override async Task<ErrorOr<ShoppingListDetailResponse>> ExecuteAsync(UpdateShoppingListEndpointRequest req, CancellationToken ct)
        => await ShoppingListService.UpdateShoppingListAsync(req.Id, User.GetUserId(), req.Body);
}
