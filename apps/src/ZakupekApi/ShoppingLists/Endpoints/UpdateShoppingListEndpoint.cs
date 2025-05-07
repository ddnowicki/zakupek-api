using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.ShoppingLists.Endpoints;

public class UpdateShoppingListEndpoint : Endpoint<UpdateShoppingListEndpointRequest, ErrorOr<bool>>
{
    public IShoppingListService ShoppingListService { get; set; } = default!;

    public override void Configure()
    {
        Put("/api/shoppinglists/{id}");
    }

    public override async Task<ErrorOr<bool>> ExecuteAsync(UpdateShoppingListEndpointRequest req, CancellationToken ct)
        => await ShoppingListService.UpdateShoppingListAsync(req.Id, User.GetUserId(), req.Body);
}
