using FastEndpoints;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public class GetShoppingListByIdRequest
{
    [BindFrom("id")]
    public int Id { get; set; }
}
