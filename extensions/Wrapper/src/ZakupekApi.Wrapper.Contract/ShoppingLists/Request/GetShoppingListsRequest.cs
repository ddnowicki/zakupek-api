using FastEndpoints;

namespace ZakupekApi.Wrapper.Contract.ShoppingLists.Request;

public class GetShoppingListsRequest
{
    [QueryParam] 
    public int Page { get; set; } = 1;
    
    [QueryParam] 
    public int PageSize { get; set; } = 10;
    
    [QueryParam] 
    public string Sort { get; set; } = "newest";
}
