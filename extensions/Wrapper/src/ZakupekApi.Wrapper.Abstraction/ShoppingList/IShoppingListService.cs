using ErrorOr;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Request;
using ZakupekApi.Wrapper.Contract.ShoppingLists.Response;

namespace ZakupekApi.Wrapper.Abstraction.ShoppingList;

public interface IShoppingListService
{
    Task<ErrorOr<ShoppingListsResponse>> GetShoppingListsAsync(
        int userId, 
        int page = 1, 
        int pageSize = 10, 
        string sort = "newest");
    
    Task<int> GetTotalShoppingListsCountAsync(int userId);
    
    Task<ErrorOr<ShoppingListDetailResponse>> GetShoppingListByIdAsync(int listId, int userId);
    
    Task<ErrorOr<ShoppingListDetailResponse>> CreateShoppingListAsync(int userId, CreateShoppingListRequest request);
    
    Task<ErrorOr<ShoppingListDetailResponse>> UpdateShoppingListAsync(int listId, int userId, UpdateShoppingListRequest request);
    
    Task<ErrorOr<bool>> DeleteShoppingListAsync(int listId, int userId);
    
    Task<ErrorOr<ShoppingListDetailResponse>> GenerateShoppingListAsync(int userId, GenerateShoppingListRequest request);
}
