using FastEndpoints;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public class DeleteShoppingListRequest
{
    [BindFrom("id")]
    public int Id { get; set; }
}
