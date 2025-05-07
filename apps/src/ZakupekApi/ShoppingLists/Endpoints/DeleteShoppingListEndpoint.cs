using ErrorOr;
using FastEndpoints;
using ZakupekApi.Wrapper.Abstraction.ShoppingList;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Users;

namespace ZakupekApi.ShoppingLists.Endpoints;

public class DeleteShoppingListEndpoint : Endpoint<DeleteShoppingListRequest, ErrorOr<bool>>
{
    public IShoppingListService ShoppingListService { get; set; } = default!;

    public override void Configure()
    {
        Delete("/api/shoppinglists/{id}");
    }

    public override async Task<ErrorOr<bool>> ExecuteAsync(DeleteShoppingListRequest req, CancellationToken ct)
        => await ShoppingListService.DeleteShoppingListAsync(req.Id, User.GetUserId());
}
